#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class IvyGrowingController : MonoBehaviour {
	//Todas las listas de variables que hacen falta para el desarrollo de una rama
	[HideInInspector]
	public List<List<Vector3>> branchs = new List<List<Vector3>>();
	public List<float> lenghts = new List<float>();
	private int currentBranch;
	[HideInInspector]
	public List<Vector3> growDirections = new List<Vector3>();
	private List<float> growDirectionsVarValue = new List<float>();
	[HideInInspector]
	public List<Vector3> grabDirections = new List<Vector3>();
	private List<float> grabDirectionsVarValue = new List<float>();

	[HideInInspector]
	public List<Leave> leaves = new List<Leave>();

	[HideInInspector]
	public int randomSeed = 26;

	//Variables públicas para definir el comportamiento del crecimiento
	public float stepSize = 0.5f;
	public float directionAmplitude = 0.2f;
	[Range (0f, 1f)]
	public float directionFrequency = 0.1f;
	[Range (0f, 1f)]
	public float directionRandomness = 0.5f;
	[Space]

	public float distanceToSurface = 0.1f;
	public float distanceToSurfaceAmplitude = 0.2f;
	[Range (0f, 1f)]
	public float distanceToSurfaceFrequency = 0.1f;
	[Space]
	public int leaveEvery = 1;
	public int randomLeaveEvery = 0;
	[Space]

	public float fallProbavilityOnCorner = 0.3f;
	public Vector3 gravity = new Vector3 (0, -1, 0);
	public float branchProbavility = 0.02f;
	public int maxBranchs = 1;

	//Variable para controlar desde fuera si el script se ejecuta o no
	public bool growing = false;

	//Variables privadas para calcular la dirección del Grab Direction
	//El Grab Direction es la dirección en la que la rama espera encontrar agarre
	private Vector3 grabDirection;
	private float grabDirectionVar = 1f;
	private float grabDirectionVarValue;

	//Esta última es para saber la última dirección de grab antes de un cambio de superficie
	// y calcular el próximo Grow Direction en consonancia
	private Vector3 oldGrabDirection;

	//Variables privadas para calcular la dirección del Grow Direction
	//El Grow Direction la dirección que la rama intentará seguir al crecer
	private Vector3 growDirection;
	private float growDirectionVarValue;


	//Variable para tener un control de las primeras iteraciones tras perder grab
	private float fallIteration = 0f;
	private List<float> fallIterations = new List<float>();
	private List<Vector3> fallVariationsDir = new List<Vector3>();
	private Vector3 fallVariationDir;

	//Variable para saber que en una búsqueda de grab no se ha encontrado nada
	private bool nothingToGrab;

	//Vectores usados para la búsqueda de superficies a las que agarrarse durante la caida
	private Vector3 searchDirection;
	private Vector3 searchDirInit;

	public float optimizeAngleBias;

	//Variable para almacenar el componente que crea la geometría para poder mandarle y leerle cosas
	private IvyGeometryCreator geom;

	void Start () {
		Random.InitState(randomSeed);
		//Inicializamos la primera branch seteando todas sus variables iniciales en sus listas
		branchs.Add (new List<Vector3>());
		lenghts.Add(0f);
		branchs[0].Add(transform.position);

		grabDirections.Add(transform.forward);
		grabDirectionsVarValue.Add(0f);

		//Elegimos una Grow Direction inicial aleatoria y perpendicular al GrabDirection
		Quaternion quat = Quaternion.AngleAxis(Random.Range(0,360), grabDirections[0]);
		growDirections.Add(Vector3.Normalize(quat * transform.right));
		growDirectionsVarValue.Add(0f);

		fallIterations.Add(0f);
		fallVariationDir = Vector3.Normalize(Vector3.Cross(gravity, Random.onUnitSphere));
		fallVariationsDir.Add(fallVariationDir);

		//Accedemos al componente geometrycreator y le mandamos el firstvector de la rama inicial
		//(que le hace falta para calcular los primeros vértices)
		geom = GetComponent<IvyGeometryCreator>();
		geom.firstVector.Add(Vector3.Normalize(Vector3.Cross(growDirections[0], grabDirections[0])));
	}

	void Update () {
	//Si el growing ha sido seteado a true, entonces es el momento de crecer
		if (growing){
			//Hacemos un for por todas las ramas existentes, lo hago de alante atras porque podrían añadirse nuevas ramas dentro del loop
			for (int i = branchs.Count - 1; i >= 0; i--){
				currentBranch = i;
				//Aquí se decide si se hace una nueva rama o no en la iteración actual
				if (Random.value <= branchProbavility && branchs.Count < maxBranchs){
					NewBranch(i);
				}
				//Las variables a la izquierda son las que usaremos para los cálculos, y las de la derecha las correspondientes a esta rama
				//Cada rama tiene que tener su set de variables de comportamiento, guardadas en listas.
				growDirection = growDirections[i];
				grabDirection = grabDirections[i];
				grabDirectionVarValue = grabDirectionsVarValue[i];
				growDirectionVarValue = growDirectionsVarValue[i];
				fallIteration = fallIterations[i];
				fallVariationDir = fallVariationsDir[i];
				grabDirectionVar = distanceToSurface + AlterateGrabDirection();
				AlterateGrowDirection();
				//En CheckFloor se comprueba si el punto actual de la rama actual tiene suelo para sustentarse y se inicia el 
				//árbol lógico
				CheckSurface (branchs[i]);
				//Aquí guardamos las variables de trabajo una vez transformadas en CheckFloor en los slots de su rama para usarlos en 
				//la siguiente iteración y que el cálculo sea contínuo
				grabDirectionsVarValue[i] = grabDirectionVarValue;
				growDirectionsVarValue[i] = growDirectionVarValue;
				fallIterations[i] = fallIteration;
				fallVariationsDir[i] = fallVariationDir;
				grabDirections[i] = grabDirection;
				growDirections[i] = growDirection;
			}
		}
	}

	void NewBranch(int i){
		branchs.Add(new List<Vector3>());
		lenghts.Add(0f);
		List<Vector3> currentBranch = branchs[i];
		List<Vector3> newBranch = branchs[branchs.Count-1];
		newBranch.Add(currentBranch[currentBranch.Count-1]);
		growDirections.Add(growDirection);
		grabDirections.Add(grabDirection);
		grabDirectionsVarValue.Add(grabDirectionVarValue);
		growDirectionsVarValue.Add(growDirectionVarValue + Mathf.PI / 2);
		fallIterations.Add(0f);
		fallVariationsDir.Add(Quaternion.AngleAxis(Random.Range(0f, 360f), gravity) * fallVariationDir);
		geom.firstVector.Add(Vector3.Normalize(Vector3.Cross(growDirection, grabDirection)));
	}

	void CheckSurface(List<Vector3> branch){
		//Aquí hacemos un Raycast en la dirección del Grab Direction para ver si hay suelo abajo
		RaycastHit RC;
		float grabDirectionMag = Vector3.Magnitude(grabDirection * grabDirectionVar);
		if (Physics.Raycast(branch[branch.Count - 1], grabDirection, out RC, 1.1f * grabDirectionMag)){
			//Si el Raycast es positivo...
			//Si estaba en plena caída y ha llegado a un nuevo suelo se ejecuta este método que randomiza la nueva GrowDirection
			if (fallIteration > 0){
				NewGrowDirectionAfterFall();
			}
			//Se pone esta variable a 0 para que en el próximo ciclo el sistema no piense que sigue cayendo
			fallIteration = 0;
			//Se setea el Grab Direction nuevo con la normal del RC...
			grabDirection = - RC.normal;
			//... Alineamos la nueva Grow Direction con la nueva superficie usando este método...
			AlignGrowDirection();
			//... Y con este método vamos a calcular el siguiente punto de la rama, y le pasamos al método 
			//el punto de impacto del raycast y la normal del mismo
			CalculateNextPoint(RC.point, RC.normal, branch);
		}
		else{
			//Si no hemos encontrado agarre donde se esperaba...
			//Se tiran los dados para saber si la rama se dejará caer o hará un intento por agarrarse a algo cercano
			if (Random.value < fallProbavilityOnCorner){
				//Este caso es si toca buscar nuevo punto de agarre
				//Esta variable sirve para calcular el nuevo GrowDirection y que no se vuelva para atrás y cosas de esas
				oldGrabDirection = grabDirection;
				//En este método se hace la búsqueda del nuevo punto de agarre
				SearchGrabPoint(branch);
			}
			//Si al tirar los dados sale que no hay que buscar agarre...
			else{
				CalculateFallPoint(branch);
				}
			}
	}

	//Este método es para calcular el próximo punto de la rama que está agarrada a una superficie
	void CalculateNextPoint(Vector3 pos, Vector3 nor, List<Vector3> branch){
		//Hacemos un RayCast en la dirección del crecimiento
		RaycastHit RC;
		float grabDirectionMag = Vector3.Magnitude(grabDirection * grabDirectionVar);
		if (!Physics.Raycast(branch[branch.Count - 1], growDirection, out RC, stepSize * 1.1f)){
			//Si no encuentra nada, añade el punto un poquíto mas para cerca del suelo, para que en la próxima comprobación del Grab no de negativo
			//Esto debería dar problemas, que estamos colocando un punto en un sitio que no a sido comprobado (0.1% de la precisión mas abajo), pero
			//me da mas problemas al arreglarlo que dejándolo así
			Vector3 newPoint = (pos + growDirection * stepSize * 0.9f) + (nor * 0.9f * grabDirectionMag);
			AddNewPoint(newPoint, branch);
		}
		else{
			//Si se encuentra un obstáculo, es que ha llegado a una nueva superficie por la que trepar, y pasamos al siguiente método
			NewSurfaceReached(-RC.normal);
		}
	}

	//Con este método se coloca el punto donde le corresponda durante una caída
	void CalculateFallPoint(List<Vector3> branch){
		Quaternion quat = Quaternion.AngleAxis(directionFrequency * 20f + Random.value * 5f, gravity);
		fallVariationDir = quat * fallVariationDir;
		growDirection = Vector3.Normalize(Vector3.Lerp(growDirection, gravity + fallVariationDir * directionAmplitude, fallIteration));
		grabDirection = growDirection;
		//Y si el growdirection se vuelve demasiado perpendicular a la gravedad...
		if (Vector3.Dot(growDirection, gravity) < 0.9f){
			//Ponemos el falliteration a 0.1 para que en la siguiente iteración, si sigue habiendo caída, vuelva a lerpear
			fallIteration = 0.1f;
		}

		//Y aumentamos el falliteration, para que en la siguiente iteración si se sigue cayendo, el growdirection sea mas parejo con 
		//la gravedad
		fallIteration += 0.1f;

		Vector3 newpoint = branch[branch.Count-1] + (growDirection * stepSize * 0.9f);
		AddNewPoint(newpoint, branch);
	}

	void SearchGrabPoint(List<Vector3> branch){
		// Vector aleatorio perpendicular a oldgrabdirection
		searchDirInit = Vector3.Normalize(Vector3.Cross(Random.onUnitSphere, grabDirection));

		//Este for sirve para hacer una búsqueda en círculo cada 45º alrededor del futur supuesto punto , a ver si habría donde agarrarse
		for (int i = 1; i < 9; i ++){
			//Aquí se crea un searchdirection en función de i		
			Quaternion quat = Quaternion.AngleAxis(i * 45, grabDirection);
			searchDirection = quat * searchDirInit;

			//Se tira el raycast desde el punto en el que debería ir el siguiente punto para ver si hay algo cerca a lo que agarrarse
			RaycastHit RC;
			if (Physics.Raycast(branch[branch.Count-1] + grabDirection * grabDirectionVar, searchDirection, out RC, stepSize * 2)){

			//Y si es así, se resetea el contador de caida, se setea el nuevo grab direction (recordar que guardamos el grabdirection anterior en la variable
			//oldgrabdirection para usarla despues, añadimos el nuevo punto de la rama donde debería estar dada su superficie, y llamamos al método que elige una nueva
			//growdirection usando la antigua grabdirection para que la rama no se de la vuelta en seco 
			fallIteration = 0;
			grabDirection = -RC.normal;
			NewGrowDirectionAfterSearch();


			branch.Add(RC.point + RC.normal * grabDirectionVar * 0.9f);
			//Ademas decimos que hemos encontrado donde agarrarnos con esta variable
			nothingToGrab = false;
			//Y rompemos el for para que no siga buscando
			break;
			}
			else{
				//Si no ha habido suerte en la búsqueda, decimos que no hemos encontrado nada
				nothingToGrab = true;
			}
		}
			//Y ya fuera del bucle, si no se ha encontrado nada, se hace lo mismo que habría pasado si no hubueramos hecho la búsqueda. Estaría bien sacarlo a un método
			if (nothingToGrab){
				//Es tiempo de caer
				CalculateFallPoint(branch);
			}
	}

	//Este método alinea la nueva Grab Direction y genera una nueva Grow Direction cuando la rama se encuentra un obstáculo de frente
	void NewSurfaceReached(Vector3 normal){
		oldGrabDirection = grabDirection;
		grabDirection = normal;
		float angle = Random.Range(-40,40);
		Quaternion quat = Quaternion.AngleAxis(angle, grabDirection);
		growDirection = quat * -oldGrabDirection;
	}

	//Este método se usa para alinear la dirección de crecimiento cada vez que se encuentra un nuevo punto de anclaje
	void AlignGrowDirection(){
		float angle = Vector3.Angle(grabDirection,growDirection);
		Quaternion quat = Quaternion.AngleAxis(90f - angle, Vector3.Cross(grabDirection, growDirection));
		growDirection = Vector3.Normalize(quat * growDirection);
	}

	//Este método es para calcular una nueva dirección de crecimiento después de una caída
	void NewGrowDirectionAfterFall(){
		//Y aquí se asigna ya el growdirection
		growDirection = Vector3.Normalize(Vector3.Cross(grabDirection, Random.onUnitSphere));
		if (Vector3.Magnitude(growDirection) == 0f){
			NewGrowDirectionAfterFall();
		}
	}

	void NewGrowDirectionAfterSearch(){
		float angle = Random.Range(-40,40);
		Quaternion quat = Quaternion.AngleAxis(angle, grabDirection);
		growDirection = quat * oldGrabDirection;
	}

	void AddNewPoint(Vector3 newPoint, List<Vector3> branch){
		if (branch.Count % (leaveEvery + Mathf.FloorToInt(Random.Range(0f, randomLeaveEvery))) == 0)
		{
			Leave l = new Leave();
			if (fallIteration == 0){
				l.right = Vector3.Normalize(Vector3.Cross(growDirection, -grabDirection));
				l.up = -grabDirection;
				l.rotation = Quaternion.LookRotation(growDirection, -grabDirection);
			}
			else{
				l.right = Vector3.Normalize(Vector3.Cross(growDirection, fallVariationDir));
				l.up = fallVariationDir;
				l.rotation = Quaternion.LookRotation(growDirection, fallVariationDir);
			}
			l.origin = newPoint;
			l.forward = growDirection;
			l.currentbranch = currentBranch;
			l.position = lenghts[currentBranch];

			leaves.Add(l);
		}

		branch.Add(newPoint);
		lenghts[currentBranch] += stepSize;
	}

	public void CleanUp(){
		fallIteration = 0f;
		fallIterations.Clear();
		fallVariationsDir.Clear();
		grabDirectionVar = 1f;
		grabDirectionVarValue = 0f;
		branchs.Clear();
		growDirections.Clear();
		grabDirections.Clear();
		grabDirectionsVarValue.Clear();
		growDirectionsVarValue.Clear();
		leaves.Clear();
	}

	void AlterateGrowDirection(){
		growDirectionVarValue += directionFrequency + (Random.value - 0.5f) * directionRandomness;
		float variation = Mathf.Sin(growDirectionVarValue) * directionAmplitude;
		Vector3 variationDir;
		variationDir = Vector3.Normalize(Vector3.Cross(growDirection, grabDirection));
		growDirection = growDirection + variationDir * variation;
	}

	float AlterateGrabDirection(){
		grabDirectionVarValue += distanceToSurfaceFrequency * 0.3f;
		float variation = (Mathf.Sin(grabDirectionVarValue) + 2) * distanceToSurfaceAmplitude/2;
		return (variation);
	}

	//Optimize y tal y cual
	public void Optimize(){
	foreach(var branch in branchs){
			for (int i = 0; i < branch.Count - 2; i++) {
				Vector3 v1 = branch[i + 1] - branch[i];
				Vector3 v2 = branch[i +2] - branch[i + 1];
				float angle = Vector3.Angle(v1, v2);
				float distance= Vector3.Distance(branch[i], branch[i + 1]);

				if (angle < optimizeAngleBias){
					branch.RemoveAt(i + 1);
				}
				if (distance < stepSize){
					branch.RemoveAt(i + 1);
				}
			}
			geom.SendMessage("Update");
		}
	}

	//Refinar las partes con angulos demasiado cerrados
	public void RefineCorners(){
		foreach(var branch in branchs){
			for (int i = 1; i < branch.Count - 1; i+=1) {
				Vector3 v1 = branch[i-1] - branch[i];
				Vector3 v2 = branch[i + 1] - branch[i];
				Vector3 old = branch[i+1];
				float angle = Vector3.Angle(Vector3.Normalize(v1), Vector3.Normalize(v2));

				if (angle < 120){
					Vector3 newPoint1 = branch[i] + v1 * 0.5f;
					Vector3 newPoint2 = branch[i] + v2 * 0.5f;

					branch[i] = newPoint2;
					branch.Insert(i, newPoint1);
				}
			}
			geom.SendMessage("Update");
		}
	}

	void OnDrawGizmos(){
	}
}

public class Leave{
	public Vector3 origin;
	public Vector3 up;
	public Vector3 forward;
	public Vector3 right;
	public Quaternion rotation;
	public int currentbranch;
	public float position;
}
#endif