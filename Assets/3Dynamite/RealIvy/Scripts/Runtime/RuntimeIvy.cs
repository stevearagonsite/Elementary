using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuntimeIvy : MonoBehaviour
{
	//Variables para almacenar los scripts de generación de geometría y comunicarnos con ellos
	RuntimeIvyGeom RTGeom;
	RuntimeIvyLeaves RTleaves;

	//Flag para desencadenar el crecimiento
	[HideInInspector]
	public bool growing;
	[HideInInspector]
	public bool createdInEditor;

	//Lista de listas de vector 3 para multirama
	[HideInInspector]
	public List<List<Vector3>> branches = new List<List<Vector3>>();
	//Lista de listas de vector 3 para las hojas (será igual que la de arriba hasta el momento en que la de arriba sea optimizada y reduzca el número de puntos)
	[HideInInspector]
	public List<List<Vector3>> leaves = new List<List<Vector3>>();
	[HideInInspector]
	public List<int> leavesCount = new List<int>();
	[HideInInspector]
	public List<List<int>> leavesPosition = new List<List<int>>();
//	public List<int> leavesCount = new List<int>();
	[HideInInspector]
//	public List<int> leaves = new List<int>();
	//Un int para acceder mas fácil al índice de la rama que estamos calculando
	int currentBranch;

	////En este bloque se almacenará la información necesaria para cada rama
	//Longitud de cada rama
	[HideInInspector]
	public List<float> lenghts = new List<float>();
	//GrowDirection de cada rama
	[HideInInspector]
	public List<Vector3> growDirections = new List<Vector3>();
	List<float> growDirectionsVarValue = new List<float>();
	//GrabDirection de cada rama
	[HideInInspector]
	public List<Vector3> grabDirections = new List<Vector3>();
	List<float> grabDirectionsVarValue = new List<float>();
	//Variables para tener un control de las primeras iteraciones tras perder grab de cada rama
	List<float> fallIterations = new List<float>();
	List<Vector3> fallVariationsDir = new List<Vector3>();
	//Variables para el control de las puntas de las ramas
	bool tipsReached = true;
	//Esta se usa también en geom para las uvs de la punta
	[HideInInspector]
	public float tipPercent;
	[HideInInspector]
	public List<Vector3> tips = new List<Vector3>();
	//Lista para el vector del primer vértice de cada loop
	[HideInInspector]
	public List<Vector3> firstVertexDirections = new List<Vector3>();
	//Lista de listas de vector 3 para los puntos de las hojas
//	public List<List<Vector3>> leaves = new List<List<Vector3>>();

	//Variables usadas en la búsqueda de nuevas superficies durante una caída
	bool nothingToGrab;
	Vector3 searchDirection;
	Vector3 searchDirInit;

	//En este bloque declaramos las variables con las que trabajaremos durante el cálculo de las ramas
	Vector3 grabDirection;
	float grabDirectionVar = 1f;
	float grabDirectionVarValue;
	Vector3 growDirection;
	float growDirectionVarValue;
	float fallIteration;
	Vector3 fallVariationDir;

	//En este bloque declaramos los parámetros de la ivy
	public int randomSeed = 26;
	public Random.State randomState;
	[Space]
	public float stepSize = 0.1f;
	public Vector3 gravity = new Vector3(0, -1, 0);
	[Space]
	public float directionAmplitude = 0.2f;
	[Range(0f, 1f)]
	public float directionFrequency = 0.1f;
	[Range(0f, 1f)]
	public float directionRandomness = 0.5f;
	[Space]
	public float distanceToSurface = 0.02f;
	public float distanceToSurfaceAmplitude = 0.1f;
	[Range(0f, 1f)]
	public float distanceToSurfaceFrequency = 0.7f;
	[Space]
	[Range(0f, 1f)]
	public float grabProbavilityOnCorner = 0.3f;
	[Space]
	public int leaveEvery = 1;
	public int randomLeaveEvery = 1;
	[Space]
	[Range(0f, 1f)]
	public float branchProbavility = 0.02f;
	[Range(1, 50)]
	public int maxBranches = 15;

	[HideInInspector]
	public float speed = 1f;
	[HideInInspector]
	public bool autoOptimize;
	[HideInInspector]
	public float optimizeAngleBias;
	bool canOptimize;
	int optimizeBranch;

	//Mas variables que no pueden ser locales
	Vector3 oldGrabDirection;
	float grabDirectionMag;
	//Esto es para que al calcular el firstvector de cada loop durante una caída, siempre salga en la misma dirección;
	Vector3 randomVector;

	float variation;
	float angle;
	Quaternion quat;
	Vector3 newPoint;
	Vector3 firstPoint;
	Vector3 variationDir;

	void Awake(){
		if (createdInEditor) {
			//Inicializamos el random
			Random.InitState (randomSeed);

			RTGeom = GetComponent<RuntimeIvyGeom> ();
			RTleaves = GetComponent<RuntimeIvyLeaves> ();

			//Creamos un vector random para usarlo mas adelante al calcular los primeros vértices de cada loop en una caída, y que sea consistente entre las ramas
			randomVector = Random.onUnitSphere;

			//Añadimos la primera rama, la primera longitud y el primer punto de la primera rama
			branches.Add (new List<Vector3> ());
			leaves.Add (new List<Vector3> ());
			leavesCount.Add (0);
			leavesPosition.Add (new List<int> ());
			lenghts.Add (0f);
			branches [0].Add (transform.position);
			//Ponemos el primer grabdirection en el forward del transform (debe estar apuntando hacia la superficia)
			grabDirections.Add (transform.forward);
			grabDirectionsVarValue.Add (0f);

			//Elegimos una Grow Direction inicial aleatoria y perpendicular al GrabDirection
			quat = Quaternion.AngleAxis (Random.Range (0, 360), grabDirections [0]);
			growDirections.Add (Vector3.Normalize (quat * transform.right));
			growDirectionsVarValue.Add (0f);

			//Elegimos una Fall Variation Direction inicial por si acaso empieza la ivy cayendo. Perpendicular a la gravedad
			fallIterations.Add (0f);
			fallVariationsDir.Add (Vector3.Normalize (Vector3.Cross (gravity, Random.onUnitSphere)));

			//Añadimos el primer tip, que estará en la posición del transform
			tips.Add (transform.position);
			//Añadimos el primer firstvertexdirection, que nos indica dónde estará el primer vértice del primer loop. Ponemos el growdirection que es la dirección que
			//mas sentido tiene de cara a la topología
			firstVertexDirections.Add (grabDirections [0]);

			RTGeom.MyStart ();
			RTleaves.MyStart ();

			randomState = Random.state;
		}
	}

	public void Initialize(){
			//Inicializamos el random
			Random.InitState (randomSeed);

			RTGeom = GetComponent<RuntimeIvyGeom> ();
			RTleaves = GetComponent<RuntimeIvyLeaves> ();

			//Creamos un vector random para usarlo mas adelante al calcular los primeros vértices de cada loop en una caída, y que sea consistente entre las ramas
			randomVector = Random.onUnitSphere;

			//Añadimos la primera rama, la primera longitud y el primer punto de la primera rama
			branches.Add (new List<Vector3> ());
			leaves.Add (new List<Vector3> ());
			leavesCount.Add (0);
			leavesPosition.Add (new List<int> ());
			lenghts.Add (0f);
			branches [0].Add (transform.position);
			//Ponemos el primer grabdirection en el forward del transform (debe estar apuntando hacia la superficia)
			grabDirections.Add (transform.forward);
			grabDirectionsVarValue.Add (0f);

			//Elegimos una Grow Direction inicial aleatoria y perpendicular al GrabDirection
			quat = Quaternion.AngleAxis (Random.Range (0, 360), grabDirections [0]);
			growDirections.Add (Vector3.Normalize (quat * transform.right));
			growDirectionsVarValue.Add (0f);

			//Elegimos una Fall Variation Direction inicial por si acaso empieza la ivy cayendo. Perpendicular a la gravedad
			fallIterations.Add (0f);
			fallVariationsDir.Add (Vector3.Normalize (Vector3.Cross (gravity, Random.onUnitSphere)));

			//Añadimos el primer tip, que estará en la posición del transform
			tips.Add (transform.position);
			//Añadimos el primer firstvertexdirection, que nos indica dónde estará el primer vértice del primer loop. Ponemos el growdirection que es la dirección que
			//mas sentido tiene de cara a la topología
			firstVertexDirections.Add (grabDirections [0]);

			RTGeom.MyStart ();
			RTleaves.MyStart ();

			randomState = Random.state;
	}

	public void GetIvyComponents(){
		RTGeom = GetComponent<RuntimeIvyGeom> ();
		RTleaves = GetComponent<RuntimeIvyLeaves> ();
	}

	//TODO la lógica para que optimize secuencialmente las ramas y se espere X frames entre optimización y optimización
	public void Update(){
		Random.state = randomState;
		if (autoOptimize && RTGeom.generateBranch) {
			if (tipPercent > 0.5f && canOptimize) {
				RTGeom.Optimize (optimizeBranch);
				optimizeBranch += 1;
				if (optimizeBranch == branches.Count) {
					optimizeBranch = 0;
				}
			}
		}
		//Si growing es true, entramos al bucle principal
		if (growing && tipsReached) {
			Step ();
			//Después del bucle de actualización de las ramas, seteamos las variables de la lógica del crecimiento de puntas al inicio
			tipsReached = false;
			tipPercent = 0f;
		}
		//Y ejecutamos el cógido para que crezcan las puntas
		GrowTip ();
		randomState = Random.state;
	}

	void GrowTip(){
		if (growing) {
			//Aumentamos el factor del lerp framerate independent
			tipPercent = Mathf.Min ( tipPercent + speed * Time.deltaTime, 1f);
			//Iteramos las ramas y desplazamos la punta con un lerp
			for (int i = 0; i < branches.Count; i++) {
				if (branches [i].Count > 1) {
					tips [i] = Vector3.Lerp (branches [i] [branches [i].Count - 2], branches [i] [branches [i].Count - 1], tipPercent);
				}
			}
			//Si hemos alcanzado el próximo punto le decimos a la lógica que lo hemos hecho para que en el siguiente frame se refresquen las ramas
			if (tipPercent >= 1) {
				tipsReached = true;
			}
		}
	}

	void Step(){
		canOptimize = true;
		//For para cada rama
		for (int i = 0; i < branches.Count; i++) {
			//Seteamos el valor de currentBranch para tener a mano el index de la rama que vamos a calcular
			currentBranch = i;
			//Decidimos si toca hacer una rama nueva y ejectuamos el método si toca
			if (Random.value <= branchProbavility && branches.Count < maxBranches && branches[currentBranch].Count > 1) {
				NewBranch (i);
			}
			//Aquí metemos en las variables de cálculo toda la información de la rama actual
			growDirection = growDirections [i];
			growDirectionVarValue = growDirectionsVarValue [i];
			grabDirection = grabDirections [i];
			grabDirectionVarValue = grabDirectionsVarValue [i];
			fallIteration = fallIterations [i];
			fallVariationDir = fallVariationsDir [i];
			//Estos dos métodos aleatorizan las grow y grab direction (la grabDirection en lo que respecta a la distancia al suelo)
			AlterateGrabDirection ();
			AlterateGrowDirection ();
			//Con este método se comprueba si la rama actual tiene apoyo y se inicia toda la lógica y todos los cálculos para que esta rama avance una iteración
			CheckSurface (branches [i]);
			//Después de todos los cálculos, ponemos a las variables de cada rama sus nuevos valores para que el cálculo sea contínuo en la siguiente iteración
			growDirections [i] = growDirection;
			growDirectionsVarValue [i] = growDirectionVarValue;
			grabDirections [i] = grabDirection;
			grabDirectionsVarValue [i] = grabDirectionVarValue;
			fallIterations [i] = fallIteration;
			fallVariationsDir [i] = fallVariationDir;

			//Mandamos a geom que cree un loop de vértices cada nuevo punto que creamos
//			Vector3 currentSegment = Vector3.forward;
//			Vector3 lastSegment = Vector3.up;
//			if (branches [currentBranch].Count > 2) {
//				currentSegment = (branches[currentBranch][branches[currentBranch].Count - 1]) - (branches[currentBranch][branches[currentBranch].Count - 2]);
//				lastSegment = (branches[currentBranch][branches[currentBranch].Count - 2]) - (branches[currentBranch][branches[currentBranch].Count - 3]);
//				Debug.Log( Vector3.Angle (currentSegment, lastSegment));
//			}
//			if (Vector3.Angle (currentSegment, lastSegment) < optimizeAngleBias) {
				if (RTGeom.generateBranch) {
					RTGeom.GenerateLoop (currentBranch);
				}
//			}
		}
	}

	void CheckSurface(List<Vector3> branch){
		//Aquí hacemos un Raycast en la dirección del Grab Direction para ver si hay suelo abajo
		RaycastHit RC;
		grabDirectionMag = Vector3.Magnitude(grabDirection * grabDirectionVar);
		if (Physics.Raycast(branch[branch.Count - 1], grabDirection, out RC, 1.1f * grabDirectionMag))
		{
			//Si el Raycast es positivo...
			//Si estaba en plena caída y ha llegado a un nuevo suelo se randomiza la nueva GrowDirection
			if (fallIteration > 0)
			{
				growDirection = Vector3.Normalize(Vector3.Cross(RC.normal, Random.onUnitSphere));
			}
			//Se pone esta variable a 0 para que en el próximo ciclo el sistema no piense que sigue cayendo
			fallIteration = 0;
			//Se setea el Grab Direction nuevo con la normal del RC...
			grabDirection = -RC.normal;
			//... Alineamos la nueva Grow Direction con la nueva superficie...
			growDirection = Vector3.Normalize (Vector3.ProjectOnPlane (growDirection, grabDirection));
			//... Y con este método vamos a calcular el siguiente punto de la rama, y le pasamos al método 
			//el punto de impacto del raycast y la normal del mismo
			CalculateNextPoint(RC.point, RC.normal, branch);
		}
		else
		{
			//Si no hemos encontrado agarre donde se esperaba...
			//Se tiran los dados para saber si la rama se dejará caer o hará un intento por agarrarse a algo cercano
			if (Random.value < grabProbavilityOnCorner)
			{
				//Este caso es si toca buscar nuevo punto de agarre
				//Esta variable sirve para calcular el nuevo GrowDirection y que no se vuelva para atrás si encuentra agarre
				oldGrabDirection = grabDirection;
				//En este método se hace la búsqueda del nuevo punto de agarre
				SearchGrabPoint(branch);
			}
			//Si al tirar los dados sale que no hay que buscar agarre...
			else
			{
				CalculateFallPoint(branch);
			}
		}
	}

	//Con esto inicializamos una nueva rama, añadiendo todo lo necesario para sus posteriores cálculos (Es básicamente como el Start())
	void NewBranch(int i){
		branches.Add(new List<Vector3>());
		leaves.Add (new List<Vector3> ());
		leavesCount.Add (0);
		leavesPosition.Add (new List<int>());
		lenghts.Add(0f);

		//Esto es para que el primer punto de la rama nueva sea el punto actual de la rama que estamos calculando
		firstPoint = branches[i][branches[i].Count-1];
		branches [branches.Count - 1].Add (firstPoint);

		growDirections.Add(growDirection);
		grabDirections.Add(grabDirection);
		grabDirectionsVarValue.Add(grabDirectionVarValue);
		growDirectionsVarValue.Add(growDirectionVarValue + Mathf.PI / 2);
		fallIterations.Add(0f);
		fallVariationsDir.Add(Vector3.Normalize(Vector3.Cross(gravity, Random.onUnitSphere)));
		tips.Add (firstPoint);
		firstVertexDirections.Add (grabDirection);


		//Al hacer una nueva rama necesitamos añadir nuevas variables a geom para la nueva rama
		RTGeom.branchesMeshes.Add(new Mesh());
		RTGeom.vertices.Add(new List<Vector3>());
		RTGeom.verticesTarget.Add (new List<Vector3>());
		RTGeom.verticesFrom.Add(new List<Vector3> ());
		RTGeom.normals.Add (new List<Vector3> ());
		RTGeom.triangles.Add (new List<int> ());
		RTGeom.uvs.Add (new List<Vector2> ());

		RTGeom.lastPointOptimized.Add (4);

		//Y creamos el primer loop a la nueva ramam indicando el índice y la dirección del primer vértice
		RTGeom.FirstLoop (branches.Count - 1, grabDirection, growDirections[growDirections.Count - 1]);

		//Y también para las hojas
		RTleaves.vertices.Add (new List<Vector3> ());
		RTleaves.uvs.Add ( new List<Vector2>());
		RTleaves.verticesTarget.Add (new List<Vector3> ());
		RTleaves.verticesFrom.Add (new List<Vector3> ());
		RTleaves.normals.Add (new List<Vector3> ());
		RTleaves.triangles.Add (new List<int> ());
		RTleaves.leavesMeshes.Add (new Mesh());
	}

	//Aquí modificamos el grabDirection en función de un seno, el distance to surface y otros parámetros
	void AlterateGrabDirection(){
		grabDirectionVarValue += distanceToSurfaceFrequency * 0.3f;
		variation = (Mathf.Sin(grabDirectionVarValue) + 2) * distanceToSurfaceAmplitude / 2;
		grabDirectionVar = distanceToSurface + variation;
	}

	//Aquí modificamos el growDirection en funcion de un seno, el grabDirection actual (para que sea alineado con la superficie) y otros parámetros 
	void AlterateGrowDirection(){
		
		growDirectionVarValue += directionFrequency + (Random.value - 0.5f) * directionRandomness;
		variation = Mathf.Sin(growDirectionVarValue) * directionAmplitude;
		variationDir = Vector3.Normalize(Vector3.Cross(growDirection, grabDirection));
		growDirection = Vector3.Normalize(growDirection + variationDir * variation);
	}

	void SearchGrabPoint(List<Vector3> branch){
		// Vector aleatorio perpendicular a grabdirection
		searchDirInit = Vector3.Normalize(Vector3.Cross(Random.onUnitSphere, grabDirection));

		//Este for sirve para hacer una búsqueda en círculo cada 45º alrededor del futuro supuesto punto , a ver si habría donde agarrarse
		for (int i = 1; i < 9; i++)
		{
			//Aquí se crea un searchdirection en función de i		
			quat = Quaternion.AngleAxis(i * 45, grabDirection);
			searchDirection = quat * searchDirInit;

			//Se tira el raycast desde el punto en el que debería ir el siguiente punto para ver si hay algo cerca a lo que agarrarse
			RaycastHit RC;
			if (Physics.Raycast(branch[branch.Count - 1] + grabDirection * grabDirectionVar, searchDirection, out RC, stepSize * 2))
			{

				//Y si es así, se resetea el contador de caida, se setea el nuevo grab direction (recordar que guardamos el grabdirection anterior en la variable
				//oldgrabdirection para usarla despues, añadimos el nuevo punto de la rama donde debería estar dada su superficie, y llamamos al método que elige una nueva
				//growdirection usando la antigua grabdirection para que la rama no se de la vuelta en seco 
				fallIteration = 0;
				grabDirection = -RC.normal;
				NewGrowDirectionAfterSearch();

				//Calculamos el nuevo punto y lo añadimos a la rama
				newPoint = RC.point + RC.normal * grabDirectionVar * 0.9f;
				AddNewPoint (newPoint, branch);

				//Ademas decimos que hemos encontrado donde agarrarnos con esta variable
				nothingToGrab = false;
				//Y rompemos el for para que no siga buscando
				break;
			}
			else
			{
				//Si no ha habido suerte en la búsqueda, decimos que no hemos encontrado nada
				nothingToGrab = true;
			}
		}
		//Y ya fuera del bucle, si no se ha encontrado nada, se hace lo mismo que habría pasado si no hubueramos hecho la búsqueda. Estaría bien sacarlo a un método
		if (nothingToGrab)
		{
			//Es tiempo de caer
			CalculateFallPoint(branch);
		}
	}

	void NewGrowDirectionAfterSearch()
	{
		angle = Random.Range(-40, 40);
		quat = Quaternion.AngleAxis(angle, grabDirection);
		growDirection = Vector3.Normalize (quat * oldGrabDirection);
	}

	//Aquí se calcula el nuevo punto en caso de que hayamos empezado una caída
	void CalculateFallPoint(List<Vector3> branch){
		//Primero se crea una rotación en torno a la gravedad para elegir hacia dónde derivará la caída (para que no caiga en línea recta)
		quat = Quaternion.AngleAxis(directionFrequency * 20f + Random.value * 5f, gravity);
		fallVariationDir = quat * fallVariationDir;
		//Lerpeamos entre la growdirection que llevaba la planta antes de perder suelo y la nueva dirección de caída durante los primeros frames para que haya transición en la caída
		growDirection = Vector3.Normalize(Vector3.Lerp(growDirection, gravity + fallVariationDir * directionAmplitude, fallIteration));
		//El nuevo growDirection se transforma en el grabDirection para buscar superficies en esa dirección en vez de en la antigua grabDirection
		grabDirection = growDirection;
		//Y si el growdirection se vuelve demasiado perpendicular a la gravedad...
		if (Vector3.Dot(growDirection, gravity) < 0.9f)
		{
			//Ponemos el falliteration a 0.1 para que en la siguiente iteración, si sigue habiendo caída, vuelva a lerpear
			fallIteration = 0.1f;
		}

		//Y aumentamos el falliteration, para que en la siguiente iteración si se sigue cayendo, el growdirection sea mas parejo con 
		//la gravedad
		fallIteration += 0.1f;

		newPoint = branch[branch.Count - 1] + (growDirection * stepSize * 0.9f);
		AddNewPoint(newPoint, branch);
	}

	//Este método sirve para añadir un nuevo punto a la rama. También actualiza la longitud de la rama que estamos calculando
	void AddNewPoint(Vector3 newPoint, List<Vector3> branch){
		branch.Add(newPoint);
		leaves [currentBranch].Add (newPoint);
		float delta = Vector3.Distance (branch [branch.Count - 2], newPoint);
		lenghts[currentBranch] += delta;
		//Si no estamos cayendo, tenemos que el firstVertexDirection será el grabdirectión
		if (fallIteration == 0f) {
			firstVertexDirections [currentBranch] = grabDirection;
		}
		//si por el contrario estamos en la caída, el firstVertexDirection tenderá a posicionarse a una dirección constante, lerpeado de modo similar al growdirection al empezar una caída
		else {
			firstVertexDirections [currentBranch] = Vector3.Normalize(Vector3.Lerp(firstVertexDirections [currentBranch], Vector3.Cross(gravity, Vector3.Lerp (growDirection, randomVector, fallIteration)), fallIteration));
		}

		if (branch.Count % (leaveEvery + Mathf.FloorToInt (Random.Range (0f, randomLeaveEvery))) == 0 && RTleaves.generateLeaves) {
			if (growDirections [currentBranch] == Vector3.zero) {
				growDirections [currentBranch] = Vector3.one;
			}

			quat = Quaternion.LookRotation (growDirections [currentBranch], -firstVertexDirections [currentBranch]);

			RTleaves.GenerateLeaves (currentBranch, quat, Random.value, Random.value, Random.value, Random.value, Random.value, Random.value, Random.Range(RTleaves.minScale, RTleaves.maxScale));
			leavesCount [currentBranch] += 1;
			leavesPosition [currentBranch].Add (leaves [currentBranch].Count - 1);
		}
	}

	//Este método alinea la nueva Grab Direction y genera una nueva Grow Direction cuando la rama se encuentra un obstáculo de frente
	Vector3 NewSurfaceReached(Vector3 point, Vector3 normal){
		//Guardamos la grabDirection para hacer que el nuevo growDirection salga en dirección opuesta al antiguo suelo
		oldGrabDirection = grabDirection;
		//la nueva grabDirection es la dirección contraria a la normal de la nueva superficie
		grabDirection = -normal;
		//El nuevo growdirection saldrá entre -40 y 40 grados con respecto a la normal de la superficie anterior en el plano de la nueva superficie
		angle = Random.Range(-40, 40);
		quat = Quaternion.AngleAxis(angle, grabDirection);
		growDirection = quat * -oldGrabDirection;
		return point - grabDirection * distanceToSurface * 0.1f;
	}

	//Este método es para calcular el próximo punto de la rama que está agarrada a una superficie (pos y nor determinan el punto y la normal de la superficie)
	void CalculateNextPoint(Vector3 pos, Vector3 nor, List<Vector3> branch){
		//Hacemos un RayCast en la dirección del crecimiento, un poquito mas allá del stepsize para que no den casualidades extrañas y traspase paredes y cosas raras
		RaycastHit RC;
		if (!Physics.Raycast(branch[branch.Count - 1], growDirection, out RC, stepSize * 1.1f))
		{
			//Si no encuentra nada, añade el punto un poquíto mas para cerca del suelo, para que en la próxima comprobación del Grab no de negativo
			//Esto debería dar problemas, que estamos colocando un punto en un sitio que no a sido comprobado (0.1% de la precisión mas abajo), pero
			//me da mas problemas al arreglarlo que dejándolo así
			newPoint = (pos + growDirection * stepSize * 0.9f) + (nor * 0.9f * grabDirectionMag);
			AddNewPoint(newPoint, branch);
		}
		else
		{
			//Si se encuentra un obstáculo, es que ha llegado a una nueva superficie por la que trepar, y pasamos al siguiente método
			AddNewPoint(NewSurfaceReached(RC.point, RC.normal), branch);
		}
	}
}