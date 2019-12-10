#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//Para que el script no funcione sin meshfilter y sin meshrenderer. Además se añaden automaticamente si no existen al añadir este
[RequireComponent (typeof(MeshFilter), typeof(MeshRenderer))]
[ExecuteInEditMode]
public class IvyGeometryCreator : MonoBehaviour {
	public bool performanceMode;
	public bool generateBranch;
	//La malla
	[HideInInspector]
	public Mesh mesh;

	private IvyGrowingController ivyPoints;
	private IvyLeavesCreator ivyLeaves;

//Variables de lo que vamos a denominar SHAPE
	//Número de lados
	[Range(3f, 16f)]
	public int sides = 6;

//Estas son las variables de lasque dependerá el radio de la IVY
	public float maxRadius = 0.05f;
	public float minRadius = 0.025f;
	//Esto hace que el radio aumente y disminuya según una función seno dependiente del punto de la ivy
	public float radiusVarFrequenzy = 0.25f;
	public float radiusVarPhase = 0f;
	//El influence es la cantidad de puntos de la ivy que entran en decaimiento para hacer la punta fina.
	public int tipInfluence = 10;
	//Esta array es el array de radios para cada punto
	private float[] radiuses;

	//Multiplicadores para las uvs
	[Range(0.01f, 10f)]
	public float uvU = 1f;
	[Range(0.01f, 1f)]
	public float uvV = 1f;

	//Variables del offset que hay que aplicar a cada vértice para colocarlos en espacio local
	public Vector3 offsetPos;
	public Quaternion offsetRot;

//Variables y matrices para el cálculo de vertices, triangulos y uvs
	private List<Vector3> vertices = new List<Vector3>();
	private List<int> triangles = new List<int>();
	private List<Vector2> uvs = new List<Vector2>();
	private float currentDistance;
	private float angle;
	private Vector3 currentSegment;
	private Vector3 currentVector;
	private Vector3 thisVector;

	public List<Vector3> firstVector = new List<Vector3>();

	CombineInstance[] combine;
	private Mesh[] meshes;

	//Variables para el modo rendimiento
	private int perfTarget = 10;
	public int perfCount = 0;

	void Start () {
		radiuses = new float[1];
		radiuses[0] = 0.5f;

	//En el start buscamos la malla y la nombramos
		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		mesh.name = "NewIvy";
		ivyPoints = GetComponent<IvyGrowingController>();
		ivyLeaves = GetComponent<IvyLeavesCreator>();

		Material mat1 = (Material)AssetDatabase.LoadAssetAtPath("Assets/Ivy.mat", typeof(Material));
		Material mat2 = (Material)AssetDatabase.LoadAssetAtPath("Assets/Ivy 1.mat", typeof(Material));

		MeshRenderer MR = GetComponent<MeshRenderer>();

		MR.sharedMaterials = new Material[2];
		Material[] newMaterials = MR.sharedMaterials;
		newMaterials[0] = mat1;
		newMaterials[1] = mat2;
		MR.sharedMaterials = newMaterials;
	}

	//Aquí está el puto meollo loco loco
	void Update () {
		if (perfCount >= perfTarget || !performanceMode){
			mesh.Clear();
			if (mesh && generateBranch){
				meshes = new Mesh[ivyPoints.branchs.Count];
				combine = new CombineInstance[meshes.Length];
				for (int i = 0; i < ivyPoints.branchs.Count; i++){
					meshes[i] = new Mesh();
					meshes[i].name = "Branch" + i;
					GenerateMesh(ivyPoints.branchs[i], meshes[i]);
					combine[i].mesh = meshes[i];
				}

				mesh.CombineMeshes(combine, true, false);
			}
			Mesh combinedBranches = Instantiate(mesh);

			CombineInstance[] combineLeaves = new CombineInstance[2];
			combineLeaves[0].mesh = combinedBranches;
			combineLeaves[1].mesh = ivyLeaves.finalMesh;
			mesh.CombineMeshes(combineLeaves, false, false);

			perfCount = 0;
		}
		else{
			perfCount ++;
		}
	}

	public void GenerateMesh(List<Vector3> branch, Mesh meshBranch){
		//Si hay por lo menos 3 puntos...
		if (branch.Count > 2){
			//Reinicializamos todo...
			InitializeStuff(branch);
			//En este for, que se hace por cada point, se calculan los vértices (posiciones y uvs) está hecho de forma que se 
			//reutilizan todos, de modo que no habrá vertices duplicados
			for (int i = 0; i < branch.Count - 1; i ++){
				//Antes de nada calculamos el radio que cada ring tendrá dependiendo del punto al que pertenezca
				CalculateRadius(i,branch);

				//Aquí vamos aumentando la distancia recorrida para las uvs verticales
				if(i > 0){
					currentDistance += Vector3.Distance(branch[i], branch[i-1]);
				}

				//Y en este bucle calculamos los n vertices de cada ring. Uno por cada side
				for ( int v = 0; v < sides; v ++){
					//Aquí calculamos el vector que va del punto al vertice
					Vector3 v1 = CalculateVector(v,i, branch);

					//Y aquí estamos cogiendo el punto en cuestión y le estamos sumando el vector calculado arriba
					//para determinar el punto del vértice. 
					vertices.Add(branch[i] + v1);

					//Y aquí calculamos las uvs
					uvs.Add(uvU * (Vector2.zero + (Vector2.right / (sides - 1) * v)) + 
					Vector2.up * currentDistance * uvV);
				}
			}
			//Aquí viene el cálculo de triángulos regulares. Lo he hecho en plan ida y vuelta
			CalculateRegularTriangles(branch);

//				Aquí colocamos el último vértice en el espacio que habíamos reservado en el array
			vertices.Add(branch[branch.Count - 1]);
			//Y no olvidemos la  uv del vertice del capuchón
			uvs.Add(Vector2.up * (currentDistance + ivyPoints.stepSize) * uvV + Vector2.right * 0.5f);

			//Aquí vienen los triángulos del capuchón, que tiene dos mecánicas para construirlos
			for (int  t = 0, c = 0; t < sides * 3; t +=3, c ++){
				if (c < sides - 1){//mecánica 1, los triángulos normales
					triangles.Add(vertices.Count - 1);
					triangles.Add(vertices.Count - 3 - c);
					triangles.Add(vertices.Count - 2 - c);
				}
				else{//mecánica 2, el último triángulo del capuchón
					triangles.Add(vertices.Count - 1);
					triangles.Add(vertices.Count - 2);
					triangles.Add(vertices.Count - 1 - sides);

				}
			}
//
			//Transformación de los vértices para colocarlos en espacio local
			for (int i = 0; i < vertices.Count; i ++){
				vertices[i] -= offsetPos;
				vertices[i] = offsetRot * vertices[i];
			}
		
			meshBranch.vertices = vertices.ToArray();
			meshBranch.triangles = triangles.ToArray();
			meshBranch.uv = uvs.ToArray();
			meshBranch.RecalculateNormals();
			meshBranch.RecalculateBounds();

			mesh.vertices = meshBranch.vertices;
			mesh.triangles = meshBranch.triangles;
			mesh.uv = meshBranch.uv;
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
//			TangentSolver tg = new TangentSolver();
//			mesh = tg.Solve(mesh);
		}
	}

	void InitializeStuff(List<Vector3> branch){
		//Limpiamos la malla anterior de haberla
		mesh.Clear();
		//Inicializamos las arrays de vértices...
		vertices.Clear();
		//de triángulos...
		triangles.Clear();
		//de uvs, de radios...
		uvs.Clear();
		radiuses = new float[branch.Count];
		//Y definimos el ángulo en función del número de caras
		angle = 2 * Mathf.PI / sides;
		//Y ponemos la distancia a 0 para empezar a contar desde 0 la próxima vez
		currentDistance = 0f;
	}

	void CalculateRadius(int i, List<Vector3> branch){
		//pasamos a float el punto para poder hacer divisiones con el
		float iFloat = i;
		//Aquí estamos calculando un radio en función de seno y de varios parámetros para conseguir un efecto ondulado
		//en el radio. Además está lerpeado entre los radios minimo y máximo
		radiuses[i] = Mathf.Sin ((iFloat / branch.Count * radiusVarFrequenzy * branch.Count) + radiusVarPhase) + 1;
		radiuses[i] = Mathf.Lerp(minRadius,maxRadius, radiuses[i]);

		radiuses[i] = Mathf.Lerp(0f, radiuses[i], Mathf.InverseLerp(0f, tipInfluence, branch.Count));

		if (i > branch.Count - tipInfluence){
				radiuses[i] = radiuses[i] * (branch.Count - iFloat) / tipInfluence;
		}
	}

	Vector3 CalculateVector(int vert, int i, List<Vector3> branch){
		Vector3 lastSegment = currentSegment;
		currentSegment = Vector3.Normalize(branch[i+1] - branch[i]);

		if (i > 0){
			Vector3 axis = Vector3.Cross(lastSegment, currentSegment);
			float angle2 = Vector3.Angle(lastSegment, currentSegment);

			Quaternion quat = Quaternion.AngleAxis(angle2, axis);
			thisVector = quat * thisVector;
		}
		else{
			thisVector = Vector3.Normalize(Vector3.Cross(firstVector[i], currentSegment));
		}

		return Quaternion.AngleAxis(Mathf.Rad2Deg * angle * vert, branch[i+ 1] - branch[i]) * 
			thisVector * radiuses[i];
	}

	void CalculateRegularTriangles(List<Vector3> branch){
		//beginBack nos indica cuando hemos cambiao el sentido
		bool beginBack = true;
		//Y esta variable la aumentamos cada vez que terminamos el loop el numero de sides, de este modo 
		//vamos incrementando las posiciones de los vértices con cada ring que hacemos
		int round = 0;
		//En el for necesitamos 3 variables. 
		//La t para la posición en el array del triangulo que estamos escribiendo
		//vert la usaremos para definir el vertice al que hace referencia. siempre acompañada de round, para que vaya saltando de ring a ring
		//c nos sirve para saber cuántos pasos hemos dado en el ring, y definir así el comportamiento de los constructores de tris, pues se dan 3 casos
		//caso ida: hacemos los triángulos normalmente hasta llegar al final del ring, entonces
		//caso vuelta: hacemos los triángulos que nos hemos dejado atrás hasta llegar al último tri del ring
		//caso último triángulo: construimos el triángulo con unas reglas diferentes
		for (int t = 0, vert = 0, c = 0; t < (branch.Count - 2) * sides * 6; t +=3, vert ++, c ++){
			if (c < sides){ //caso ida
				triangles.Add(vert + round);
				triangles.Add(vert + 1 + round);
				triangles.Add(vert + sides + round);
			}
			else{ //caso vuelta (aun pueden darse dos condiciones dependiedo del valor de c
				if (beginBack){ //cambiamos el vert para que la vuelta se calcule bien
					vert = 1;
					beginBack = false;
				}
				if (c < (sides * 2) - 1){ //caso vuelta estandar
					triangles.Add(vert + round);
					triangles.Add(vert + sides + round);
					triangles.Add(vert + (sides - 1) + round);
				}
				else{//caso último triángulo
					triangles.Add(vert - sides + round);
					triangles.Add(sides + round);
					triangles.Add((vert - sides) + (sides - 1) + round);

					//reinicializamos todo y aumentamos round en sides para que el siguiente loop referencie a los vertices
					//del siguiente ring
					vert = -1;
					beginBack = true;
					c = -1;
					round += sides;
				}
			}
		}
	}

	public void GenerateLightmapUvs(){
		if (mesh){
			Unwrapping.GenerateSecondaryUVSet(mesh);
		}
	}

	public void CleanUp(){
		combine = null;
		meshes = null;
		firstVector.Clear();
		vertices.Clear();
		triangles.Clear();
		uvs.Clear();
		if (mesh){
			mesh.Clear();
		}
	}

	void OnDrawGizmos(){

	}
}
#endif