using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuntimeIvyGeom : MonoBehaviour {
	//Para acceder a los datos de la ivy
	RuntimeIvy ivy;
	RuntimeIvyLeaves RTLeaves;
	//Y meshrenderer y demás
	MeshFilter MF;



	//Listas para vértices y toda la info geométrica de las mallas
	[HideInInspector]
	public List<List<Vector3>> vertices =  new List<List<Vector3>>();
	[HideInInspector]
	public List<List<Vector3>> verticesTarget = new List<List<Vector3>>();
	[HideInInspector]
	public List<List<Vector3>> verticesFrom = new List<List<Vector3>>();
	[HideInInspector]
	public List<List<Vector3>> normals = new List<List<Vector3>> ();
	[HideInInspector]
	public List<List<int>> triangles = new List<List<int>> ();
	[HideInInspector]
	public List<List<Vector2>> uvs = new List<List<Vector2>> ();
	[HideInInspector]
	public List<Mesh> branchesMeshes = new List<Mesh>();
	[HideInInspector]
	public List<int> lastPointOptimized = new List<int>();

	//float entre 0 y 1 que indica en qué punto de crecimiento está la enredadera entre un punto y el siguiente
	float tipPercent;
	//este ángulo será el ángulo que habrá que girar el firstVertexDirection para crear todos lod vértices
	float angle;
	//El segmento que está creciendo actualmente
	Vector3 currentSegment;
	Vector3 lastSegment;
	//La malla final y el combineInstance para mezclar todas las branchmeshes
	Mesh mesh;
	CombineInstance[] combine;
	CombineInstance[] combineLeaves;
	//Esto sirve para el optimize, para contabilizar los loops removidos y no descuadrar la contabilidad de vértices
	int removedLoops;

	//Parámetros para el usuario
	//Flag para generaro no las branches
	[HideInInspector]
	public bool generateBranch = true;
	public float maxRadius = 0.05f;
	public float minRadius = 0.025f;
	[Space]
	[HideInInspector]
	public float radius;
	public float radiusVarFrequenzy = 0.25f;
	public float radiusVarPhase = 0f;
	[Space]
	[Range(3,20)]
	public int sides = 6;
	[Space]
	[Range(0,10)]
	public float uvH = 1f;
	[Range(0,10)]
	public float uvV = 1f;
	[Space]
	//es el número de loops hacia atrás desde la punta que se van afilando
	public int tipInfluence = 1;

	float localAngle;
	float distance;
	float factor;
	int branchPoint;
	int loopCount;
	Quaternion quat;
	Vector3 vertex;
	Vector3 tipVertex;
	Vector3 from;
	Vector3 segment1;
	Vector3 segment2;
	List<Vector3> branch;

	Quaternion localRotationCorrection;

	// Use this for initialization
	public void MyStart () {
		//Cogemos el componente RuntimeIvy para acceder a las variables de las ramas
		ivy = GetComponent<RuntimeIvy> ();
		RTLeaves = GetComponent<RuntimeIvyLeaves> ();
		//Y los meshrenderer y meshfilter
		MF = GetComponent<MeshFilter> ();

		//Añadimos una nueva malla y listas para almacenar la primera rama
		branchesMeshes.Add(new Mesh());
		verticesTarget.Add (new List<Vector3> ());
		vertices.Add(new List<Vector3> ());
		verticesFrom.Add(new List<Vector3> ());
		normals.Add (new List<Vector3> ());
		triangles.Add (new List<int> ());
		uvs.Add(new List<Vector2>());

		lastPointOptimized.Add (5);

		//Calculamos el ángulo que habrá que girar el firstVertexDirection para crear todos lod vértices
		angle = 2 * Mathf.PI / sides;

		localRotationCorrection = Quaternion.Inverse (transform.rotation);
		//Creamos el primer loop de la primera rama, dandole los parámetros necesarios
		FirstLoop (0, ivy.firstVertexDirections[0], ivy.growDirections[0]);


		//Y creamos la malla y el combine para combinar las hojas y las ramas y la asignamos en el MF
		mesh = new Mesh ();
		mesh.name = "Runtime Ivy Mesh";
		combineLeaves = new CombineInstance[2];
		MF.mesh = mesh;
	}
	
	// Update is called once per frame
	public void Update () {
		if (ivy.growing && generateBranch) {
			//Este for es para actualizar todas las ramas.
			for (int currentBranch = 0; currentBranch < ivy.branches.Count; currentBranch++) {
				//La posición y UV del último vértice
				vertex = ivy.tips [currentBranch];
				//Esto es para transformar a espacio local
				vertex -= transform.position;
				vertex = localRotationCorrection * vertex;

				vertices [currentBranch] [vertices [currentBranch].Count - 1] = vertex;
//				normals [currentBranch] [normals [currentBranch].Count - 1] =  Quaternion.Inverse (transform.rotation) *  ivy.growDirections [currentBranch];

				//Actualizamos la UV de la punta tambíen, además de la posicion, en el centro horizontal, y en el vertical a la altura del penúltimo punto + tippercent
				uvs [currentBranch] [uvs [currentBranch].Count - 1] = new Vector2 (0f, uvs[currentBranch][uvs[currentBranch].Count - 3].y + ivy.tipPercent);
				//El estrechamiento en la punta se hace con este método
				SharpTip (currentBranch);
				//Y actualizamos las 
				UpdateSubMesh (currentBranch);
			}			

			//Y lo último del update es juntar todas las subMeshes en una sola, incluídas las de las hojas
		}
		if (ivy.growing) {
			if (generateBranch || RTLeaves.generateLeaves) {
				CombineMeshes();
			}
		}
	}

	//El método para sacarle punta a la punta. Como argumento le metemos la rama que vamos a afilar. este método es llamado desde un loop que las recorre todas.
	void SharpTip(int currentBranch){
		//Este for lo que hace es recorrer desde el penúltimo vértice una rama, hasta que llegamos al último vértice del último loop afectado por la punta
		for (int i = vertices[currentBranch].Count-2; i > (vertices[currentBranch].Count-2) - ((sides + 1) * tipInfluence); i--){
			//Con esto rompemos el bucle para no meternos en indices negativos los primeros frames de una rama
			if (i < 0)
				break;
			//Aquí sacamos a qué punto de la rama pertenece el vértice
			branchPoint = Mathf.CeilToInt ((i + 1f) / (sides + 1f));
			//El factor es un número que nos dice en cuál de los puntos estamos respecto a la punta de la rama (mayor, mas lejos de la punta)
			factor = (branchPoint - ivy.branches [currentBranch].Count + 1) * -1;
			//Y nos viene bien para sumárselo al tipPercent, de forma que a los vértices mas lejanos a la punta se les sume mas que a los mas cercanos, de manera proporcional
			//Y al dividirlo por tipInfluence (que es el número de loops afectados por el estrechamiento en la punta, se normaliza el valor.
			//Así conseguimos un valor entre 0 y 1 proporcional y asignado a cada vértice según a qué loop pertenezca
			tipPercent = (ivy.tipPercent + factor) / tipInfluence;

			//Este sería el punto del que tienen que partir los vértices en su transición (con su transformación a espacio local)
			from = verticesFrom[currentBranch][i] ; //ivy.branches [currentBranch] [branchPoint - 1];
			from -= transform.position;
			from = localRotationCorrection * from;
			//Y aquí lo lerpeamos con su posición final, usando el tipPercent
			vertices [currentBranch] [i] = Vector3.Lerp (from, verticesTarget [currentBranch] [i], tipPercent);
		}
	}

	public void GenerateLoop(int currentBranch){
		//Guardamos en una variable local la rama que vamos a trabajar para tenerlo mas a mano
		branch = ivy.branches [currentBranch];

		if (branch.Count > 2) {
			//Calculamos el currentSegment para posteriores cálculos
			currentSegment = branch [branch.Count - 1] - branch [branch.Count - 2];

			//Ejecutámos el método para calcular el ŕadio que nos toca
			CalculateRadius (currentBranch);

			//Con un for dependiendo del número de caras...
			for (int v = 0; v <= sides; v++) {
				//creamos los vértices y los insertamos en sus respectivas listas según la rama...
				quat = Quaternion.AngleAxis (Mathf.Rad2Deg * angle * v, currentSegment);
				vertex = Vector3.Normalize (quat * ivy.firstVertexDirections [currentBranch]);				

				normals [currentBranch].Insert (normals[currentBranch].Count - 1, localRotationCorrection * vertex);

				vertex = (vertex * radius + branch [branch.Count - 2]);					

				//Conversión a espacio local
				vertex -= transform.position;
				vertex = localRotationCorrection * vertex;
				
				//Metemos en ensta lista la posición final
				verticesTarget [currentBranch].Add (vertex);
				verticesFrom[currentBranch].Add(ivy.branches[currentBranch][ivy.branches[currentBranch].Count - 2]);
				//Y en esta otra la posición inicial (el centro del tallo). Esta es la lista que vamos a pasar directamente a la malla, es por eso que 
				//insertamos en el penúltimo hueco en vez de añadir, porque en esta lista está el último vértice (la punta) y en la lista anterior solo hay vértices de tallo
				vertices[currentBranch].Insert(vertices[currentBranch].Count - 1, ivy.branches[currentBranch][ivy.branches[currentBranch].Count - 1]);										
			}

			//UVs no tiene mucha explicación, se calculan y se meten igual que los vértices pero tiramos de leaves para que al optimizar y perder puntos en branches
			//no afecte a la colocación de uvs
			for (int v = 0; v <= sides; v++) {
				uvs [currentBranch].Insert (uvs[currentBranch].Count - 1, new Vector2(uvH * v, (ivy.leaves[currentBranch].Count - 1) * uvV));
			}

			//Con este método reasignamos los vértices a los triángulos del capuchón según creamos nuevos loops
			AlterateTapTriangles (currentBranch);

			loopCount = (vertices [currentBranch].Count / (sides + 1)) - 2 ;

			//Si tenemos tallo y no solo punta, se calculan los tris.
			if (branch.Count > 2) {
				CalculateTriangles (currentBranch, loopCount);
			}
			//Y updateamos las submeshes
			UpdateSubMesh (currentBranch);
		}
	}

	//Este es el algoritmo de triangulación.
	void CalculateTriangles(int currentBranch, int round){
		for (int i = 0; i < sides; i++) {
			triangles[currentBranch].Add (i + (round * (sides + 1)));
			triangles[currentBranch].Add (i + (round * (sides + 1)) + 1);
			triangles[currentBranch].Add (i + (round * (sides + 1)) + sides + 1);

			triangles[currentBranch].Add (i + (round * (sides + 1)) + 1);
			triangles[currentBranch].Add (i + (round * (sides + 1)) + sides + 2);
			triangles[currentBranch].Add (i + (round * (sides + 1)) + sides + 1);
		}
	}

	//Aquí estamos cogiendo los primeros triángulos (del capuchón) y asignándoles los nuevos vértices que le tocan en cada loop
	void AlterateTapTriangles(int currentBranch) {		
		for (int i = 0; i < sides * 3; i += 3){
			triangles[currentBranch] [i] += sides + 1;
			triangles[currentBranch] [i+1] += sides + 1;
			triangles[currentBranch] [i+2] += sides + 1;
		}
	}

	 //Con este método creamos el primer loop de una branch. Necesitamos el índice de la rama para meter los vértices en la lista correcta, la dirección del primer vértice y el eje de rotación del loop
	public void FirstLoop(int currentBranch, Vector3 firstVertexDirection, Vector3 axis){
		//Metemos la rama actual en una variable para tenerla mas a mano
		List<Vector3> branch = ivy.branches [currentBranch];
		//Ejecutámos el método para calcular el ŕadio que nos toca
		CalculateRadius (currentBranch);

			//Un bucle para cada cara donde cogemos el firstVertexDirection y lo rotamos respecto al axis para conseguir los demás vértices del loop
		for (int v = 0; v <= sides; v++) {
			quat = Quaternion.AngleAxis (Mathf.Rad2Deg * angle * v, axis);
			vertex = Vector3.Normalize ( quat * firstVertexDirection);
			normals[currentBranch].Add (localRotationCorrection * vertex);
			vertex = (vertex * radius + branch [0]);

			//Transformación a espacio local
			vertex -= transform.position;
			vertex = localRotationCorrection * vertex;
			//Metemos la posición final de los vértices en esta lista
			verticesTarget [currentBranch].Add (vertex);
			//Y aquí la posición actual de los vértices, que es el punto de la rama (el centro del tallo)
			vertices[currentBranch].Add(ivy.branches[currentBranch][ivy.branches[currentBranch].Count - 1]);
			verticesFrom[currentBranch].Add(ivy.branches[currentBranch][ivy.branches[currentBranch].Count - 1]);
		}
		//Este vértice es el de la punta (el centro del tallo justo en la posición en la que nace la rama que estamos creando)
		tipVertex = ivy.branches[currentBranch][ivy.branches[currentBranch].Count - 1];

		normals[currentBranch].Add ( localRotationCorrection * ivy.growDirections [currentBranch]);

		//Transformación a espacio local
		tipVertex -= transform.position;
		tipVertex = localRotationCorrection * tipVertex;
		vertices[currentBranch].Add (tipVertex);

		//UVs del primer loop, no tiene mucha explicación
		for (int v = 0; v <= sides; v++) {
			uvs [currentBranch].Add (new Vector2(uvH * v, 0f));
		}
		uvs [currentBranch].Add (new Vector2(uvH * sides/2f, 0f));

		//Y los primeros triángulos. Estos van a ser siempre los primeros de la lista de tris, y los tris del tallo se van añadiendo después.
		for (int c = 0; c < sides; c++) {
			triangles[currentBranch].Add (c);
			triangles[currentBranch].Add (c + 1);
			triangles[currentBranch].Add (vertices[currentBranch].Count-1);
		}
		//Actualizamos las submeshes
		UpdateSubMesh (currentBranch);
	}
	//Calculo del radio en cada loop. No tiene mucho misterio
	void CalculateRadius(int currentBranch){
		radius = Mathf.Sin ((ivy.lenghts[currentBranch] * radiusVarFrequenzy) + radiusVarPhase) + 1;
		radius = Mathf.Lerp(minRadius,maxRadius, radius);
	}
	//Aquí metemos la información calculada en las mallas
	void UpdateSubMesh(int currentBranch){
		branchesMeshes [currentBranch].Clear ();
		branchesMeshes [currentBranch].vertices = vertices[currentBranch].ToArray ();
		branchesMeshes [currentBranch].triangles = triangles[currentBranch].ToArray ();
		branchesMeshes [currentBranch].normals = normals [currentBranch].ToArray ();
		branchesMeshes [currentBranch].uv = uvs [currentBranch].ToArray ();
	}

	public void Optimize(int i){		
		removedLoops = 0;
		for (int p = lastPointOptimized [i]; p < ivy.branches [i].Count - 1 - tipInfluence; p++) {
			segment1 = ivy.branches [i] [p - 1] - ivy.branches [i] [p - 2];
			segment2 = ivy.branches [i] [p - 2] - ivy.branches [i] [p - 3];
			localAngle = Vector3.Angle (segment1, segment2);

		if (localAngle < ivy.optimizeAngleBias) {
				RemoveLoop (i, p - 1);
				Optimize (i);
				lastPointOptimized [i] = Mathf.Max( (ivy.branches [i].Count - tipInfluence), 5);
				break;
			}
		}
	}

	void RemoveLoop(int i, int p){
		ivy.branches [i].RemoveAt (p);
		for (int t = 0; t < sides; t++) {
			triangles [i].RemoveAt ((triangles [i].Count - 1));
			triangles [i].RemoveAt ((triangles [i].Count - 1));
			triangles [i].RemoveAt ((triangles [i].Count - 1));
			triangles [i].RemoveAt ((triangles [i].Count - 1));
			triangles [i].RemoveAt ((triangles [i].Count - 1));
			triangles [i].RemoveAt ((triangles [i].Count - 1));
		}

		for (int ti = 0; ti < sides * 3; ti += 3) {
			triangles [i] [ti] -= sides + 1;
			triangles [i] [ti + 1] -= sides + 1;
			triangles [i] [ti + 2] -= sides + 1;
		}

		for (int v = 0; v < sides + 1; v++) {
			int point = p - 2;
			vertices [i].RemoveAt ((point - removedLoops) * (sides + 1));
			verticesTarget [i].RemoveAt ((point - removedLoops) * (sides + 1));
			verticesFrom [i].RemoveAt ((point - removedLoops) * (sides + 1));
			normals [i].RemoveAt ((point - removedLoops) * (sides + 1));
			uvs [i].RemoveAt ((point - removedLoops) * (sides + 1));
		}

		removedLoops += 1;
	}

	void CombineMeshes(){
		combine = new CombineInstance[branchesMeshes.Count];
		for (int i = 0; i < combine.Length; i++) {
				combine [i].mesh = branchesMeshes [i];
		}
		mesh.CombineMeshes (combine, true, false);

		Mesh combinedBranches = Instantiate(mesh);

		combineLeaves [0].mesh = combinedBranches;
		combineLeaves [1].mesh = RTLeaves.mesh;

		mesh.CombineMeshes (combineLeaves, false, false);
	}

	void OnDrawGizmos(){

	}
}