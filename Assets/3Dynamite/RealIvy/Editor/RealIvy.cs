#if UNITY_EDITOR
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;

public class RealIvy : EditorWindow {

	//VARIABLES DE DEARROLLO ORDENAR Y LIMPIAR
	Vector3 lastDrawPoint;
	bool drawing = false;
	bool btDraw;
	int drawBranchCounter = 0;
	Vector3 nearestPoint;
	bool overLastPoint = false;
	bool overOnePoint = false;
	int overBranchIndex;
	int drawingBranch;
	Rect rectUndo;
	bool drawMode = true;
	Texture2D newIcon;

	//Aquí guardamos el gameobject de la enredadera así como los componentes que lleva
	GameObject newIvy;
	IvyGrowingController newIvyPoints;
	IvyGeometryCreator newIvyGeom;
	IvyLeavesCreator newIvyLeaves;
	ProceduralIvy newIvyProc;
	MeshRenderer meshRenderer;

	//Aquí declaramos el snapshot por si guardamos la ivy generada
	GameObject snapShot;

	//Variables de lógica de comportamiento de la herramienta
	bool growing = false;
	bool firstIteration = true;
	bool placing = false;
	bool performanceMode = false;
	bool generateLightmapUvs = false;
	float optimizeAngleBias = 10f;

	//Variables donde se almacena la información del raycast sobre la sceneview al colocar la ivy
	Vector3 mouseRCPoint = Vector3.zero;
	Vector3 mouseRCNormal = Vector3.zero;

	//Variables para GUI
	float gizmoSize = 1f;

	float windowHeight = 800;
	float columnWidth = 320;

	Vector2 toolBarScroll = Vector2.zero;
	Vector2 paramsScroll = Vector2.zero;
	Vector2 presetsScroll = Vector2.zero;

	Vector2 buttonSize = new Vector2(265, 32);
	Vector2 buttonPos = new Vector2(0,40);
	Vector2 halfButtonSize = new Vector2(128,32);
	Vector2 halfButtonSize2 = new Vector2(128,16);
	Vector2 halfButtonPos = new Vector2(138, 0);

	Vector2 fieldSize = new Vector2(265,16);
	Vector2 fieldPos = new Vector2(0, 24);

	Vector2 lineSize = new Vector2(210,3);
	Vector2 linePosH = new Vector2(30, 0);
	Vector2 linePosV = new Vector2(0, 24);

	Vector2 presetPosH = new Vector2(5, 0);
	Vector2 presetPosV = new Vector2(0, 30);
	Vector2 presetSize = new Vector2(150, 20);
	Vector2 presetButtonSize = new Vector2(85, 20);
	Vector2 presetButtonSize2 = new Vector2(20, 20);
	Vector2 presetButtonSize3 = new Vector2(90, 20);

	string newPresetName;
	string presetsPath = "Assets/3Dynamite/RealIvy/Presets";
	List<Preset> presets = new List<Preset>();
	int presetTarget;
	int presetSelected;
	bool confirmAction;
	bool overwritting;

	bool btCreateAtCenter;
	bool btPlacing;
	bool btStart;
	bool btPerformance;
	bool btStep;
	bool btReset;
	bool btRandomize;
	bool btOptimize;
	bool btRefineCorners;
	bool btCancel;
	bool btSnapshot;

	GUISkin realIvyGuiskin;
	GUIStyle realIvyButton;
	GUIStyle realIvyButtonOrange;
	GUIStyle realIvyButtonGreen;
	GUIStyle realIvyButtonRed;
	GUIStyle realIvyButtonBlue;
	GUIStyle realIvyLabel;
	GUIStyle realIvyTitle;
	GUIStyle realIvyTabOn;
	GUIStyle realIvyTabOff;

	Color bgColor;
	Color bgColor2;
	Color bgColor3;

	//La variable de parámetros de la ivy
	IvyParameters newIvyParameters = new IvyParameters();

[MenuItem ("Tools/3Dynamite/Real Ivy")]
	//Se crea la ventana y se guarda en la variable para añadirle parámetros
	public static void Init(){
		RealIvy myWindow = (RealIvy)EditorWindow.GetWindow(typeof (RealIvy));
		myWindow.maxSize = new Vector2(1280, 800);
		myWindow.minSize = new Vector2(320, 0);
		myWindow.Show();
		myWindow.autoRepaintOnSceneChange = true;
		//Nos suscribimos un evento de teclas modificadoras... no sé para qué
		EditorApplication.modifierKeysChanged += myWindow.Repaint;
	}

	void Awake(){
		//Aquí definimos todos los estilos de gui para la ventana
		realIvyGuiskin =  (GUISkin)AssetDatabase.LoadAssetAtPath("Assets/3Dynamite/RealIvy/Guiskins/RealIvyGuiskin.guiskin", typeof(GUISkin));
		newIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3Dynamite/RealIvy/Guiskins/Icons/New_Icon.psd", typeof(Texture2D));
		realIvyButton = new GUIStyle(realIvyGuiskin.GetStyle("button"));
		realIvyButtonOrange = new GUIStyle(realIvyGuiskin.GetStyle("buttonorange"));
		realIvyButtonGreen = new GUIStyle(realIvyGuiskin.GetStyle("buttongreen"));
		realIvyButtonRed = new GUIStyle(realIvyGuiskin.GetStyle("buttonred"));
		realIvyButtonBlue = new GUIStyle(realIvyGuiskin.GetStyle("buttonblue"));
		realIvyLabel = new GUIStyle(realIvyGuiskin.GetStyle("label"));
		realIvyTitle = new GUIStyle(realIvyGuiskin.GetStyle("textarea"));
		realIvyTabOn = new GUIStyle(realIvyGuiskin.GetStyle("tabon"));
		realIvyTabOff = new GUIStyle(realIvyGuiskin.GetStyle("taboff"));

		//Cargamos presets...
		LoadPresets();
		//Y si se carga al menos uno, la gui se queda con los parámetros del primero
		if (presets.Count > 0){
			newIvyParameters = presets[0].presetParameters;
		}
	}

	 //Nos suscribimos al evento de OnSceneGUI (Nos desuscribimos antes por si acaso ya estábamos suscritos)
	void OnFocus() {
	    SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
	    SceneView.onSceneGUIDelegate += this.OnSceneGUI;

		if (!newIvy) {
			CleanUp ();
		}
	 }

	 //Al cerrar la ventana, nos desuscribimos del evento OnSceneGUI y limpiamos todo
	 void OnDestroy() {
		CleanUp();
	    SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
	 }

	 //Aquí ya se complica el tema
	 void OnSceneGUI(SceneView sceneView) {
	 	//Aquí almacenamos el evento, que creo que es lo que está sucediendo en cada momento
	 	Event current = Event.current;
	 	//Y esto es el ID de la ventana que servirá para que mientras esta ventana está abierta, el comportamiento de la escena cambie
	 	int controlID = GUIUtility.GetControlID(FocusType.Passive);

	 	//Aquí creamos los botones de undo, y si el ratón está encima de ellos, entonces no va a haber ninguna lógica de crear ivy
	 	//así podemos pulsar con seguridad de que no interferirá el click con la enredadera
	 	//en caso contrario, se ejectua la lógica normal del scenegui
		bool undoButton = false;
	 	if (drawing){
//			rectUndo = new Rect(10, 10, 100, 50);
			rectUndo = new Rect(new Vector2 (sceneView.position.width / 2f - 50f, sceneView.position.height - 80f), new Vector2 (100f, 50f));
			Handles.BeginGUI();
			undoButton = GUI.Button(rectUndo, "Undo", realIvyButtonOrange);
			Handles.EndGUI();
		}
		if(rectUndo.Contains(Event.current.mousePosition)){
			if (undoButton){
				Debug.Log("Undo performed");
				drawBranchCounter -= 1;
				if (drawBranchCounter == -1) {
					CleanUp ();
				} 
				else {
					newIvyPoints.branchs.RemoveAt (newIvyPoints.branchs.Count - 1);
					newIvyPoints.leaves.RemoveAll (a => a.currentbranch == newIvyPoints.branchs.Count);
				}
			}
		}
		else{

		Handles.color = Color.blue;
		if (newIvy && drawing){
			if (overLastPoint){
				Handles.color = Color.yellow;
			}
			else{
				Handles.color = Color.blue;
			}
			if (overOnePoint){
				Handles.DrawSolidDisc(nearestPoint, sceneView.camera.transform.forward, newIvyParameters.stepSize / 3f);
			}
		}

		//Esto es para el raycast, si el ratón se mueve por la sceneview, hacemos un raycast y guardamos los valores que nos interesan
		if (Event.current.type == EventType.MouseMove){
			RayCastSceneView();

			//Mierda de desarrollo de draw
			if (newIvy){
				overLastPoint = false;
				for (int o = 0; o < newIvyPoints.branchs.Count; o++) {
					for (int i = 0; i < newIvyPoints.branchs[o].Count; i++) {
						if (Vector3.Distance(mouseRCPoint, newIvy.transform.rotation * 	newIvyPoints.branchs[o][i] + newIvy.transform.position) < newIvyParameters.stepSize){
							nearestPoint = newIvy.transform.rotation * newIvyPoints.branchs[o][i] + newIvy.transform.position;
							overOnePoint = true;
							overBranchIndex = o;
							if (i == newIvyPoints.branchs[o].Count - 1){
								overLastPoint = true;
							}
							else{
								overLastPoint = false;
							}
							return;
						}
						else{
							overOnePoint = false;
						}
					}
				}
			}
		}

		//Este switch es para el evento de hacer click
		switch (current.type){
			//Si el evento actual es mouseUp, entonces hace to la pesca de crear el ivy en su sitio y demás
			case EventType.MouseUp:{
				if (placing && current.button == 0 && !current.alt){
					CreateIvy(mouseRCPoint, mouseRCNormal);
					placing = !placing;
					break;
				}
				//DESARROLLO DRAWING
				if (drawing && current.button == 0 && !current.alt){
					break;
				}
				break;
			}
			//DESARROLLO DRAWING
			case EventType.MouseDown:{
				if (!newIvy){
					if (drawing && current.button == 0 && !current.alt){
						CreateIvy(mouseRCPoint, mouseRCNormal);
						lastDrawPoint = mouseRCPoint;
						break;
					}
				}
				else{
					if (drawing && current.button == 0 && !current.alt){
						if (!overLastPoint){
							newIvyPoints.branchs.Add(new List<Vector3>());
							newIvyPoints.lenghts.Add(0f);
							drawBranchCounter += 1;
							drawingBranch = drawBranchCounter;
						}
						if (overLastPoint){
							drawingBranch = overBranchIndex;
						}
						else{
							if (overOnePoint){
								newIvyPoints.branchs[drawingBranch].Add(Quaternion.Inverse(newIvy.transform.rotation) * (nearestPoint - newIvy.transform.position));
							}
						
							else{
								newIvyPoints.branchs[drawingBranch].Add(Quaternion.Inverse(newIvy.transform.rotation) * (mouseRCPoint - newIvy.transform.position));
							}
						}
						break;
					}
				}
				break;
			}
			//DESARROLLO DRAWING
			case EventType.MouseDrag:{
				if ((placing || drawing) && current.button == 0 && !current.alt && newIvyPoints){
					DrawIvy();
					break;
				}
				break;
			}
			//Aquí dice que mientras estña la ventana dibujandose o algo similar, quien tiene el control del sceneview es esta ventana (creo)
			case EventType.Layout:
				if (placing || drawing){
					HandleUtility.AddDefaultControl(controlID);
					break;
				}
				break;
			}

		//Si estamos en proceso de colocar el Ivy en la escena, se dibuja el gizmo donde le corresponde
		if (placing){
			Handles.DrawLine(mouseRCPoint + mouseRCNormal * newIvyParameters.distanceToSurface * 0.1f * 0.8f, (mouseRCPoint + mouseRCNormal * (newIvyParameters.distanceToSurface + newIvyParameters.distanceToSurfaceAmplitude * 2) * 0.8f) + mouseRCNormal * gizmoSize/2 * 3);
			Handles.DrawWireDisc(mouseRCPoint + mouseRCNormal * newIvyParameters.distanceToSurface * 0.1f, mouseRCNormal, gizmoSize/2);
			Handles.color = new Color (0.3f,0.3f,1f,0.3f);
			Handles.DrawSolidDisc(mouseRCPoint + mouseRCNormal * (newIvyParameters.distanceToSurface * 0.1f) * 0.8f, mouseRCNormal, gizmoSize/2);
		}

		if (drawing){
			Handles.color = new Color (0f,0f,0f,1f);
				Handles.DrawWireDisc(mouseRCPoint + mouseRCNormal * (newIvyParameters.distanceToSurface + newIvyParameters.maxRadius), mouseRCNormal, newIvyParameters.stepSize);
			Handles.color = new Color (1f,1f,1f,1f);
				Handles.DrawWireDisc(mouseRCPoint + mouseRCNormal * (newIvyParameters.distanceToSurface + newIvyParameters.maxRadius), mouseRCNormal, newIvyParameters.stepSize * 1.2f);
		}
	 }
	 }

	//DESARROLLO DRAWING
	 void DrawIvy(){
		RayCastSceneView();
		Vector3 growDirection = mouseRCPoint - lastDrawPoint;
		newIvyPoints.branchs[0][0] = Vector3.zero;
		if (Vector3.Distance(mouseRCPoint, lastDrawPoint) > newIvyParameters.stepSize){
			newIvyPoints.branchs[drawingBranch].Add(Quaternion.Inverse(newIvy.transform.rotation) * (mouseRCPoint + mouseRCNormal * newIvyParameters.maxRadius - newIvy.transform.position));
			newIvyPoints.lenghts[drawingBranch] += newIvyParameters.stepSize;
			lastDrawPoint = mouseRCPoint;
			if (newIvyPoints.branchs[drawingBranch].Count % (newIvyParameters.leaveEvery + Mathf.FloorToInt(UnityEngine.Random.Range(0f, newIvyParameters.leaveEveryRandomness))) == 0)
			{
				Leave l = new Leave();
				l.right = Vector3.Normalize(Vector3.Cross(growDirection, mouseRCNormal));
				l.up = mouseRCNormal;
				l.rotation = Quaternion.LookRotation(growDirection, mouseRCNormal);

				l.origin = mouseRCPoint + mouseRCNormal * newIvyParameters.maxRadius;
				l.forward = growDirection;
				l.currentbranch = drawingBranch;
				l.position = newIvyPoints.lenghts[drawingBranch];
				newIvyPoints.leaves.Add(l);
			}
		}
	 }

	 void RayCastSceneView(){
		Vector2 guiPosition = Event.current.mousePosition;
		Ray ray = HandleUtility.GUIPointToWorldRay(guiPosition);
		RaycastHit RC;
		Physics.Raycast(ray, out RC);
		mouseRCPoint = RC.point + RC.normal * newIvyParameters.distanceToSurface;
		mouseRCNormal = RC.normal;
	 }

	 //Creamos el Ivy
	 void CreateIvy(Vector3 pos, Vector3 nor){
		if (!newIvy){
	 	//Si se ha creado via placing, el comportamiento es un poco diferente
	 	if (placing){
			pos += nor * (newIvyParameters.distanceToSurface * 0.1f);
	 	}
		 	//Se crea el objeto y se coloca en su sitio y se le atribuye todo lo que se tiene que atribuir, incluido el iconito
			newIvy = new GameObject();
			newIvy.transform.position = pos;
			newIvy.transform.LookAt (pos - nor);
			newIvy.name = "New Ivy Seed";
			AssignLabel(newIvy);
			//Y se selecciona
			Selection.activeGameObject = newIvy;
			//Guardamos en una variable el componente de Ivy del objeto creado
			newIvyPoints = newIvy.AddComponent<IvyGrowingController>();
			newIvyGeom = newIvy.AddComponent<IvyGeometryCreator>();
			newIvyLeaves = newIvy.AddComponent<IvyLeavesCreator>();
			newIvyProc = newIvy.AddComponent < ProceduralIvy >();

			newIvyPoints.hideFlags = HideFlags.HideInInspector;
			newIvyGeom.hideFlags = HideFlags.HideInInspector;
			newIvyLeaves.hideFlags = HideFlags.HideInInspector;
//
			meshRenderer = newIvy.GetComponent<MeshRenderer>();

			//Seteamos las variables del ivy con los parámetros de la ventana
			SendParameters();
		}
		else{
			Debug.Log("You must create Ivys one by one. Cancel the current ivy before.");
		}
	 }

	void OnGUI () {
		bgColor = new Color(0.6f, 0.61f, 0.62f);
		bgColor2 = new Color(0.55f, 0.56f, 0.57f);
		bgColor3 = new Color(0.7f, 0.7f, 0.7f);
		EditorGUI.DrawRect(new Rect(Vector2.zero, position.size), bgColor);

		toolBarScroll = GUI.BeginScrollView(new Rect(0, 0, columnWidth , position.height), toolBarScroll, new Rect(0, 0, columnWidth -16, windowHeight), false, false);
			EditorGUI.DrawRect(new Rect(columnWidth - 8, 0, 2, position.height), Color.gray);

			//Materials and mesh
			EditorGUI.LabelField(new Rect(1, 175, columnWidth - 28, 25), "Materials and Mesh", realIvyTitle);
			GUI.BeginGroup(new Rect(7, 170, columnWidth,100));
			EditorGUI.LabelField(new Rect(16, 38, columnWidth/2 - 32, 20), "Branches Material", realIvyLabel);
			newIvyParameters.branchMaterial = EditorGUI.ObjectField(new Rect(columnWidth/2, 40, columnWidth/2 - 32, 16), newIvyParameters.branchMaterial, typeof(Material), false) as Material;
			if (newIvyParameters.branchMaterial)
				newIvyParameters.branchMaterialGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(newIvyParameters.branchMaterial.GetInstanceID()));
			else newIvyParameters.branchMaterialGUID = null;
			EditorGUI.LabelField(new Rect(16, 58, columnWidth/2 - 32, 20), "Leaves Material", realIvyLabel);
			newIvyParameters.leavesMaterial = EditorGUI.ObjectField(new Rect(columnWidth/2, 60, columnWidth/2 - 32, 16), newIvyParameters.leavesMaterial, typeof(Material), false) as Material;
			if (newIvyParameters.leavesMaterial)
			newIvyParameters.leavesMaterialGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(newIvyParameters.leavesMaterial.GetInstanceID()));
			else newIvyParameters.leavesMaterialGUID = null;
			EditorGUI.LabelField(new Rect(16, 78, columnWidth/2 - 32, 20), "Leaves Mesh", realIvyLabel);
			newIvyParameters.leavesMesh = EditorGUI.ObjectField(new Rect(columnWidth/2, 80, columnWidth/2 - 32, 16), newIvyParameters.leavesMesh, typeof(Mesh), false) as Mesh;
			if (newIvyParameters.leavesMesh)
			newIvyParameters.leavesMeshGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(newIvyParameters.leavesMesh.GetInstanceID()));
			else newIvyParameters.leavesMeshGUID = null;
			GUI.EndGroup();

		EditorGUI.LabelField(new Rect(1, 8, columnWidth - 28, 25), "Presets", realIvyTitle);
		EditorGUI.DrawRect(new Rect(5, 40, columnWidth-25, 98), Color.gray); 
		GUI.BeginGroup(new Rect(5, 30, columnWidth, 140));
		if (!confirmAction){
			presetsScroll = GUI.BeginScrollView(new Rect(0, 16, 290, 84), presetsScroll, new Rect(0, 0, 250, 30 * presets.Count), false, false);
			for (int i = 0; i < presets.Count; i++){
			if (presetSelected == i){
				if (GUI.Button(new Rect(presetPosH + presetPosV * i, presetSize), presets[i].presetName, realIvyButtonGreen)){
					presets.Clear();
					LoadPresets();
					newIvyParameters = presets[i].presetParameters;
					presetSelected = i;
				}
			}
			else{
				if (GUI.Button(new Rect(presetPosH + presetPosV * i, presetSize), presets[i].presetName, realIvyButton)){
					presets.Clear();
					LoadPresets();
					newIvyParameters = presets[i].presetParameters;
					presetSelected = i;
				}
			}
				if (presets[i].deletable){
					if (GUI.Button(new Rect(presetPosH * 32 + presetPosV * i, presetButtonSize ), "Overwrite", realIvyButtonBlue)){
						presetTarget = i;
						overwritting = true;
						confirmAction = true;
					}
					if (GUI.Button(new Rect(presetPosH * 50 + presetPosV * i, presetButtonSize2 ), "X", realIvyButtonRed)){
						presetTarget = i;
						confirmAction = true;
					}
				}
			}
			GUI.EndScrollView();
			newPresetName = EditorGUI.TextField(new Rect(presetPosH + presetPosV * 3.8f, presetSize), newPresetName);
			if (GUI.Button(new Rect(presetPosH * 35 + presetPosV * 3.8f, presetButtonSize3 ), "New Preset", realIvyButtonBlue)){
					NewPreset(newPresetName);
				}
		}
		else{
			EditorGUI.DrawRect(new Rect(5, 15, columnWidth - 40, 84), bgColor);
			GUI.Label(new Rect(20,20,200,30), "Are you sure?", realIvyTitle);
			if (GUI.Button(new Rect(30, 60, 100, 30), "Yes",realIvyButton)){
				if (!overwritting){
					DeletePreset(presetTarget);
				}
				else{
					presets[presetTarget].presetParameters = newIvyParameters;
					SavePresets();
					overwritting = false;
				}
				confirmAction = false;
			}
			if (GUI.Button(new Rect(160, 60, 100, 30), "Cancel",realIvyButton)){
				confirmAction = false;
			}
		}
		GUI.EndGroup();

			//Controls
			EditorGUI.LabelField(new Rect(1, 275, columnWidth - 28, 25), "Controls", realIvyTitle);
		if (drawMode) {
			EditorGUI.DrawRect(new Rect(10, 342, columnWidth - 30, 200), bgColor3);
		}
		else {
			EditorGUI.DrawRect(new Rect(10, 342, columnWidth - 30, 260), bgColor3);
		}
			GUI.BeginGroup(new Rect(24, 270, columnWidth - 5, 550));
		if (drawMode) {
			if (GUI.Button (new Rect (buttonPos, halfButtonSize), "Paint", realIvyTabOn)) {
			}
			GUI.DrawTexture (new Rect (0, 20, 50, 50), newIcon);
			if (GUI.Button (new Rect (buttonPos + halfButtonPos, halfButtonSize), "Procedural", realIvyTabOff)) {
				if (newIvy) {
					Debug.Log ("You cannot switch the mode during the ivy creation. Cancel to switch the creation mode.");
				}
				else {
					drawMode = false;
					drawing = false;
				}
			}
			//DESARROLLO DRAWING
			if (!drawing){
				btDraw = GUI.Button(new Rect(buttonPos * 2.5f, buttonSize), "Start Painting", realIvyButtonBlue);
			}
			else{
				btDraw = GUI.Button(new Rect(buttonPos * 2.5f, buttonSize), "Stop Painting", realIvyButtonOrange);
			}
			GUI.Label(new Rect(new Vector2(20, 148), new Vector2(200, 100)), "(These properties are shared with");
			GUI.Label(new Rect(new Vector2(20, 162), new Vector2(200, 100)), "procedural mode)");
			newIvyParameters.stepSize = EditorGUI.FloatField(new Rect(buttonPos * 4.5f, fieldSize), "Step Size", newIvyParameters.stepSize);
			newIvyParameters.distanceToSurface = EditorGUI.FloatField(new Rect(buttonPos * 5f, fieldSize), "Distance to surface", newIvyParameters.distanceToSurface);
			GUI.Label(new Rect(buttonPos * 5.75f, new Vector2(270, 28)), "More painting features soon!", realIvyTitle);
		} 
		else {
			if (GUI.Button (new Rect (buttonPos, halfButtonSize), "Paint", realIvyTabOff)) {
				if (newIvy) {
					Debug.Log ("You cannot switch the mode during the ivy creation. Cancel to switch the creation mode.");
				}
				else {
					drawMode = true;
					placing = false;
				}
			}
			GUI.DrawTexture (new Rect (0, 20, 50, 50), newIcon);
			if (GUI.Button (new Rect (buttonPos + halfButtonPos, halfButtonSize), "Procedural", realIvyTabOn)) {
			}

			btCreateAtCenter = GUI.Button(new Rect(buttonPos * 2.1f, buttonSize), "Create ivy at center of Scene View", realIvyButtonBlue);

			if (!placing){
				btPlacing = GUI.Button(new Rect(buttonPos * 3.1f, buttonSize), "Place by clicking on Scene View", realIvyButtonBlue);
			}
			else{
				btPlacing = GUI.Button(new Rect(buttonPos * 3.1f, buttonSize), "Cancel placing", realIvyButtonOrange);
			}

			if (!growing){
				btStart = GUI.Button(new Rect(buttonPos * 4.1f, buttonSize), "Start Growth", realIvyButtonGreen);
			}
			else{
				btStart = GUI.Button(new Rect(buttonPos * 4.1f, buttonSize), "Stop Growth", realIvyButtonOrange);
			}

			if (!performanceMode){
				btPerformance = GUI.Button(new Rect(buttonPos * 5.1f, buttonSize), "Performance Mode is OFF", realIvyButton);
			}
			else{
				btPerformance = GUI.Button(new Rect(buttonPos * 5.1f, buttonSize), "Performance Mode is ON", realIvyButtonOrange);
			}

			btStep = GUI.Button(new Rect(buttonPos * 6.1f, halfButtonSize), "Step", realIvyButton);
			btReset = GUI.Button(new Rect(buttonPos * 6.1f + halfButtonPos, halfButtonSize), "Reset", realIvyButton);

			btRandomize = GUI.Button(new Rect(buttonPos * 7.1f, buttonSize), "Randomize", realIvyButton);
		}

				btOptimize = GUI.Button(new Rect(buttonPos * 8.5f, halfButtonSize), "Optimize", realIvyButton);
				optimizeAngleBias = EditorGUI.Slider(new Rect(buttonPos * 8.85f + halfButtonPos, halfButtonSize2), optimizeAngleBias, 0f, 30f);
				EditorGUI.LabelField(new Rect(buttonPos * 8.5f + halfButtonPos, halfButtonSize2), "Angle Bias");
				btRefineCorners = GUI.Button(new Rect(buttonPos * 9.5f, buttonSize), "Refine Corners", realIvyButton);
				btCancel = GUI.Button(new Rect(buttonPos * 10.5f, halfButtonSize), "Cancel", realIvyButtonRed);
				btSnapshot = GUI.Button(new Rect(buttonPos * 10.5f + halfButtonPos, halfButtonSize), "Snapshot", realIvyButtonGreen);
				generateLightmapUvs = GUI.Toggle(new Rect(buttonPos * 11.4f + halfButtonPos, halfButtonSize), generateLightmapUvs, "               ");
				EditorGUI.LabelField(new Rect(buttonPos * 11.4f + halfButtonPos * 1.1f, halfButtonSize), "Generate Lightmap");
				EditorGUI.LabelField(new Rect(buttonPos * 11.8f + halfButtonPos * 1.1f, halfButtonSize), "UVs");
				gizmoSize = EditorGUI.Slider(new Rect(buttonPos * 12.4f, fieldSize), "Gizmo Size", gizmoSize, 0f, 2f);
			GUI.EndGroup();

		GUI.EndScrollView();

		if (position.width > columnWidth + 80){
			if (!drawMode){
				paramsScroll = GUI.BeginScrollView (new Rect (columnWidth + 16, 0, position.width - (columnWidth + 16), position.height), paramsScroll, new Rect (0, 0, columnWidth * 3 - 32, windowHeight - 16), false, false);
				EditorGUI.LabelField (new Rect (2, 8, columnWidth - 40, 25), "Procedural Growth Parameters", realIvyTitle);
				EditorGUI.DrawRect (new Rect (10, 45, columnWidth - 40, 145), bgColor2);
				EditorGUI.DrawRect (new Rect (10, 220, columnWidth - 40, 80), bgColor2);
				EditorGUI.DrawRect (new Rect (10, 330, columnWidth - 40, 100), bgColor2);
				EditorGUI.DrawRect (new Rect (10, 460, columnWidth - 40, 55), bgColor2);
				EditorGUI.LabelField (new Rect (columnWidth - 17, 8, columnWidth - 20, 25), "Branches Shape Parameters", realIvyTitle);
				EditorGUI.DrawRect (new Rect (columnWidth - 7, 45, columnWidth - 40, 190), bgColor2);
				EditorGUI.DrawRect (new Rect (columnWidth - 7, 270, columnWidth - 40, 60), bgColor2);

				EditorGUI.LabelField (new Rect (columnWidth * 2 - 17, 8, columnWidth - 20, 25), "Leaves Parameters", realIvyTitle);
				EditorGUI.DrawRect (new Rect (columnWidth * 2 - 7, 45, columnWidth - 40, 220), bgColor2);
				if (!newIvyParameters.globalDirection) {
					EditorGUI.DrawRect (new Rect (columnWidth * 2 - 7, 290, columnWidth - 40, 110), bgColor2);
				} else {
					EditorGUI.DrawRect (new Rect (columnWidth * 2 - 7, 290, columnWidth - 40, 140), bgColor2);
				}

				GUI.BeginGroup (new Rect (16, 24, columnWidth, 500));
				newIvyParameters.stepSize = EditorGUI.FloatField (new Rect (fieldPos * 1, fieldSize), "Step Size", newIvyParameters.stepSize);
				newIvyParameters.gravity = EditorGUI.Vector3Field (new Rect (fieldPos * 2, fieldSize), "Gravity", newIvyParameters.gravity);
				newIvyParameters.maxBranchs = EditorGUI.IntSlider (new Rect (fieldPos * 3.6f, fieldSize), "Max Branches", newIvyParameters.maxBranchs, 1, 50);
				newIvyParameters.branchProvability = EditorGUI.Slider (new Rect (fieldPos * 4.5f, fieldSize), "Branch Provability", newIvyParameters.branchProvability, 0f, 1f);
				EditorGUI.LabelField (new Rect (fieldPos * 5.5f, fieldSize), "Grab provability when");
				EditorGUI.LabelField (new Rect (fieldPos * 6f, fieldSize), "falling");
				newIvyParameters.fallProbavilityOnCorner = EditorGUI.Slider (new Rect (fieldPos * 5.7f, fieldSize), "                       ", newIvyParameters.fallProbavilityOnCorner, 0f, 1f);

				EditorGUI.DrawRect (new Rect (linePosH + linePosV * 7.5f, lineSize), Color.gray);

				newIvyParameters.directionAmplitude = EditorGUI.FloatField (new Rect (fieldPos * 8.5f, fieldSize), "Direction Amplitude  ", newIvyParameters.directionAmplitude);
				newIvyParameters.directionFrequenzy = EditorGUI.Slider (new Rect (fieldPos * 9.5f, fieldSize), "Direction Frequency", newIvyParameters.directionFrequenzy, 0f, 1f);
				newIvyParameters.directionRandomness = EditorGUI.Slider (new Rect (fieldPos * 10.5f, fieldSize), "Direction Randomness", newIvyParameters.directionRandomness, 0f, 1f);

				EditorGUI.DrawRect (new Rect (linePosH + linePosV * 12f, lineSize), Color.gray);

				newIvyParameters.distanceToSurface = EditorGUI.FloatField (new Rect (fieldPos * 13f, fieldSize), "Distance to surface", newIvyParameters.distanceToSurface);
				EditorGUI.LabelField (new Rect (fieldPos * 14f, fieldSize), "Distance to surface");
				EditorGUI.LabelField (new Rect (fieldPos * 14.5f, fieldSize), "amplitude");
				newIvyParameters.distanceToSurfaceAmplitude = EditorGUI.FloatField (new Rect (fieldPos * 14.2f, fieldSize), "                   ", newIvyParameters.distanceToSurfaceAmplitude);
				EditorGUI.LabelField (new Rect (fieldPos * 15.5f, fieldSize), "Distance to surface");
				EditorGUI.LabelField (new Rect (fieldPos * 16f, fieldSize), "frequency");
				newIvyParameters.distanceToSurfaceFrequenzy = EditorGUI.Slider (new Rect (fieldPos * 15.7f, fieldSize), "                    ", newIvyParameters.distanceToSurfaceFrequenzy, 0f, 1f);

				EditorGUI.DrawRect (new Rect (linePosH + linePosV * 17.5f, lineSize), Color.gray);

				newIvyParameters.leaveEvery = EditorGUI.IntField (new Rect (fieldPos * 18.5f, new Vector2 (200, 16)), "Create leave every", newIvyParameters.leaveEvery);
				EditorGUI.LabelField (new Rect (new Vector2 (0, 24) * 18.5f + new Vector2 (200, 0), new Vector2 (40, 16)), " steps");
				newIvyParameters.leaveEveryRandomness = EditorGUI.IntField (new Rect (fieldPos * 19.5f, new Vector2 (200, 16)), "Randomize              + -", newIvyParameters.leaveEveryRandomness);
				GUI.EndGroup ();

				GUI.BeginGroup(new Rect(columnWidth, 24, columnWidth, 500));
					newIvyParameters.generateBranch = EditorGUI.Toggle(new Rect(fieldPos * 1f, fieldSize), "Generate branches", newIvyParameters.generateBranch);
					newIvyParameters.sides = EditorGUI.IntSlider(new Rect(fieldPos * 2f , fieldSize), "Sides", newIvyParameters.sides, 3, 20);
					newIvyParameters.maxRadius = EditorGUI.FloatField(new Rect(fieldPos * 3f, fieldSize), "Max. Radius", newIvyParameters.maxRadius);
					newIvyParameters.minRadius = EditorGUI.FloatField(new Rect(fieldPos * 4f, fieldSize), "Min. Radius", newIvyParameters.minRadius);
					newIvyParameters.tipInfluenceBranch = EditorGUI.IntField(new Rect(fieldPos * 5f , fieldSize), "Tip influence", newIvyParameters.tipInfluenceBranch);

					EditorGUI.LabelField(new Rect(fieldPos * 6f, fieldSize), "Radius variation");
					EditorGUI.LabelField(new Rect(fieldPos * 6.5f, fieldSize), "frequency");
					newIvyParameters.radiusVariationFrequenzy = EditorGUI.FloatField(new Rect(fieldPos * 6.2f , fieldSize), "                   ", newIvyParameters.radiusVariationFrequenzy);

					EditorGUI.LabelField(new Rect(fieldPos * 7.5f, fieldSize), "Radius variation");
					EditorGUI.LabelField(new Rect(fieldPos * 8f, fieldSize), "phase");
					newIvyParameters.radiusVariationPhase = EditorGUI.FloatField(new Rect(fieldPos * 7.7f , fieldSize), "                   ", newIvyParameters.radiusVariationPhase);

				EditorGUI.DrawRect(new Rect(linePosH + linePosV * 9.5f,lineSize), Color.gray);

					newIvyParameters.uvU = EditorGUI.Slider(new Rect(fieldPos * 10.5f, fieldSize), "Horizontal UV", newIvyParameters.uvU, 0f, 10f);
					newIvyParameters.uvV = EditorGUI.Slider(new Rect(fieldPos * 11.5f, fieldSize), "Vertical UV", newIvyParameters.uvV, 0f, 10f);
				GUI.EndGroup();

				GUI.BeginGroup(new Rect(columnWidth * 2, 24, columnWidth, 500));
					newIvyParameters.generateLeaves = EditorGUI.Toggle(new Rect(fieldPos * 1f, fieldSize), "Generate leaves", newIvyParameters.generateLeaves);
					newIvyParameters.maxScale = EditorGUI.FloatField(new Rect(fieldPos * 2f, fieldSize), "Max. Scale", newIvyParameters.maxScale);
					newIvyParameters.minScale = EditorGUI.FloatField(new Rect(fieldPos * 3f, fieldSize), "Min. Scale", newIvyParameters.minScale);
					newIvyParameters.tipInfluenceLeave = EditorGUI.FloatField(new Rect(fieldPos * 4f , fieldSize), "Tip influence", newIvyParameters.tipInfluenceLeave);

				EditorGUI.DrawRect(new Rect(linePosH + linePosV * 5.5f,lineSize), Color.gray);

					newIvyParameters.positionOffset = EditorGUI.Vector3Field(new Rect(fieldPos * 6.5f, fieldSize), "Position offset", newIvyParameters.positionOffset);
					newIvyParameters.positionOffsetRandomness = EditorGUI.Vector3Field(new Rect(fieldPos * 8.3f, fieldSize), "Position offset randomness", newIvyParameters.positionOffsetRandomness);

				EditorGUI.DrawRect(new Rect(linePosH + linePosV * 10.5f,lineSize), Color.gray);

					newIvyParameters.globalDirection = EditorGUI.Toggle(new Rect(fieldPos * 11.5f, fieldSize), "Global orientation", newIvyParameters.globalDirection);

					if (newIvyParameters.globalDirection){
						newIvyParameters.globalDirectionInfluence = EditorGUI.Slider(new Rect(fieldPos * 12.5f, fieldSize), "Global orientation influence", newIvyParameters.globalDirectionInfluence, 0f, 1f);
						newIvyParameters.globalDirectionVector = EditorGUI.Vector3Field(new Rect(fieldPos * 13.5f, fieldSize), "Global Direction", newIvyParameters.globalDirectionVector);
						newIvyParameters.globalDirectionVectorRandomness = EditorGUI.Vector3Field(new Rect(fieldPos * 15.2f, fieldSize), "Global Direction Randomness", newIvyParameters.globalDirectionVectorRandomness);
					}
					else{
						newIvyParameters.directionVector = EditorGUI.Vector3Field(new Rect(fieldPos * 12.5f, fieldSize), "Direction", newIvyParameters.directionVector);
						newIvyParameters.directionVectorRandomness = EditorGUI.Vector3Field(new Rect(fieldPos * 14f, fieldSize), "Direction randomness", newIvyParameters.directionVectorRandomness);
					}
				GUI.EndGroup();
				EditorGUI.DrawRect (new Rect (0, 0, 2, position.height), Color.gray);
				EditorGUI.DrawRect(new Rect(columnWidth - 19f, 0, 2f, position.height), Color.gray);
				EditorGUI.DrawRect(new Rect(columnWidth * 2f - 19f, 0, 2f, position.height), Color.gray);
			GUI.EndScrollView();//Parameters
			}
			else{
				paramsScroll = GUI.BeginScrollView (new Rect (columnWidth + 16, 0, position.width - (columnWidth + 16), position.height), paramsScroll, new Rect (0, 0, columnWidth * 2 - 32, windowHeight - 16), false, false);
				EditorGUI.LabelField (new Rect (0, 8, columnWidth - 35, 25), "Branches Shape Parameters", realIvyTitle);
				EditorGUI.DrawRect (new Rect (10, 45, columnWidth - 40, 190), bgColor2);
				EditorGUI.DrawRect (new Rect (10, 270, columnWidth - 40, 60), bgColor2);

				EditorGUI.LabelField (new Rect (columnWidth - 17, 8, columnWidth - 35, 25), "Leaves Parameters", realIvyTitle);
				EditorGUI.DrawRect (new Rect (columnWidth - 7, 45, columnWidth - 40, 220), bgColor2);
				if (!newIvyParameters.globalDirection) {
					EditorGUI.DrawRect (new Rect (columnWidth - 7, 290, columnWidth - 40, 110), bgColor2);
				} else {
					EditorGUI.DrawRect (new Rect (columnWidth - 7, 290, columnWidth - 40, 140), bgColor2);
				}

				GUI.BeginGroup(new Rect(17, 24, columnWidth, 500));
				newIvyParameters.generateBranch = EditorGUI.Toggle(new Rect(fieldPos * 1f, fieldSize), "Generate branches", newIvyParameters.generateBranch);
				newIvyParameters.sides = EditorGUI.IntSlider(new Rect(fieldPos * 2f , fieldSize), "Sides", newIvyParameters.sides, 3, 20);
				newIvyParameters.maxRadius = EditorGUI.FloatField(new Rect(fieldPos * 3f, fieldSize), "Max. Radius", newIvyParameters.maxRadius);
				newIvyParameters.minRadius = EditorGUI.FloatField(new Rect(fieldPos * 4f, fieldSize), "Min. Radius", newIvyParameters.minRadius);
				newIvyParameters.tipInfluenceBranch = EditorGUI.IntField(new Rect(fieldPos * 5f , fieldSize), "Tip influence", newIvyParameters.tipInfluenceBranch);

				EditorGUI.LabelField(new Rect(fieldPos * 6f, fieldSize), "Radius variation");
				EditorGUI.LabelField(new Rect(fieldPos * 6.5f, fieldSize), "frequency");
				newIvyParameters.radiusVariationFrequenzy = EditorGUI.FloatField(new Rect(fieldPos * 6.2f , fieldSize), "                   ", newIvyParameters.radiusVariationFrequenzy);

				EditorGUI.LabelField(new Rect(fieldPos * 7.5f, fieldSize), "Radius variation");
				EditorGUI.LabelField(new Rect(fieldPos * 8f, fieldSize), "phase");
				newIvyParameters.radiusVariationPhase = EditorGUI.FloatField(new Rect(fieldPos * 7.7f , fieldSize), "                   ", newIvyParameters.radiusVariationPhase);

				EditorGUI.DrawRect(new Rect(linePosH + linePosV * 9.5f,lineSize), Color.gray);

				newIvyParameters.uvU = EditorGUI.Slider(new Rect(fieldPos * 10.5f, fieldSize), "Horizontal UV", newIvyParameters.uvU, 0f, 10f);
				newIvyParameters.uvV = EditorGUI.Slider(new Rect(fieldPos * 11.5f, fieldSize), "Vertical UV", newIvyParameters.uvV, 0f, 10f);
				GUI.EndGroup();

				GUI.BeginGroup(new Rect(columnWidth, 24, columnWidth, 500));
				newIvyParameters.generateLeaves = EditorGUI.Toggle(new Rect(fieldPos * 1f, fieldSize), "Generate leaves", newIvyParameters.generateLeaves);
				newIvyParameters.maxScale = EditorGUI.FloatField(new Rect(fieldPos * 2f, fieldSize), "Max. Scale", newIvyParameters.maxScale);
				newIvyParameters.minScale = EditorGUI.FloatField(new Rect(fieldPos * 3f, fieldSize), "Min. Scale", newIvyParameters.minScale);
				newIvyParameters.tipInfluenceLeave = EditorGUI.FloatField(new Rect(fieldPos * 4f , fieldSize), "Tip influence", newIvyParameters.tipInfluenceLeave);

				EditorGUI.DrawRect(new Rect(linePosH + linePosV * 5.5f,lineSize), Color.gray);

				newIvyParameters.positionOffset = EditorGUI.Vector3Field(new Rect(fieldPos * 6.5f, fieldSize), "Position offset", newIvyParameters.positionOffset);
				newIvyParameters.positionOffsetRandomness = EditorGUI.Vector3Field(new Rect(fieldPos * 8.3f, fieldSize), "Position offset randomness", newIvyParameters.positionOffsetRandomness);

				EditorGUI.DrawRect(new Rect(linePosH + linePosV * 10.5f,lineSize), Color.gray);

				newIvyParameters.globalDirection = EditorGUI.Toggle(new Rect(fieldPos * 11.5f, fieldSize), "Global orientation", newIvyParameters.globalDirection);

				if (newIvyParameters.globalDirection){
					newIvyParameters.globalDirectionInfluence = EditorGUI.Slider(new Rect(fieldPos * 12.5f, fieldSize), "Global orientation influence", newIvyParameters.globalDirectionInfluence, 0f, 1f);
					newIvyParameters.globalDirectionVector = EditorGUI.Vector3Field(new Rect(fieldPos * 13.5f, fieldSize), "Global Direction", newIvyParameters.globalDirectionVector);
					newIvyParameters.globalDirectionVectorRandomness = EditorGUI.Vector3Field(new Rect(fieldPos * 15.2f, fieldSize), "Global Direction Randomness", newIvyParameters.globalDirectionVectorRandomness);
				}
				else{
					newIvyParameters.directionVector = EditorGUI.Vector3Field(new Rect(fieldPos * 12.5f, fieldSize), "Direction", newIvyParameters.directionVector);
					newIvyParameters.directionVectorRandomness = EditorGUI.Vector3Field(new Rect(fieldPos * 14f, fieldSize), "Direction randomness", newIvyParameters.directionVectorRandomness);
				}
				GUI.EndGroup();
				EditorGUI.DrawRect (new Rect (0, 0, 2, position.height), Color.gray);
				EditorGUI.DrawRect(new Rect(columnWidth - 19f, 0, 2f, position.height), Color.gray);
				EditorGUI.DrawRect(new Rect(columnWidth * 2f - 19f, 0, 2f, position.height), Color.gray);
				GUI.EndScrollView();//Parameters
			}
		}
	}

	//Método pillado de por internet gracias a DelzHand
	public static void AssignLabel(GameObject g)
	  {
		Texture2D tex = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3Dynamite/RealIvy/SampleAssets/Textures/Leave_Icon.png", typeof(Texture2D));
	    Type editorGUIUtilityType  = typeof(EditorGUIUtility);
	    BindingFlags bindingFlags = BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic;
	    object[] args = new object[] {g, tex};
	    editorGUIUtilityType.InvokeMember("SetIconForObject", bindingFlags, null, null, args);
	  }

	//En el update...
	void Update(){
		//Mandamos a las sceneview que se refresquen para que lo que suceda se vea en tiempo real
		SceneView.RepaintAll();
		if (newIvy){
			SendParameters();
			//Si hemos empezado el crecimiento dandole a crecer...
			if (growing){
				//Si es la primera iteración llamamos al método start de la ivy y desactivamos la baliza
				if (firstIteration){
					FirstIteration();
				}
				//En cualquier caso, despues, llamamos al update
//				newIvyPoints.SendMessage("Update");
			}
		}
		if (btCreateAtCenter){
				CreateAtCenter();
		}
		if (btPlacing){
			Place();
		}
		if (btStart){
			Grow();
		}
		if (btStep){
			Step();
		}
		if (btReset){
			Reset();
		}
		if (btRandomize){
			Randomize();
		}
		if (btPerformance){
			PerformanceMode();
		}
		if (btOptimize){
			if (newIvy){
				newIvyPoints.Optimize();
			}
		}
		if (btRefineCorners){
			if (newIvy){
				newIvyPoints.RefineCorners();
			}
		}
		//DESARROLLO DRAWING
		if (btDraw){
			Draw();
		}
		if (btCancel){
			CleanUp();
		}
		if (btSnapshot){
			Snapshot();
		}
	}

	void CreateAtCenter(){
		CreateIvy(SceneView.lastActiveSceneView.camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 1f)) + SceneView.lastActiveSceneView.camera.transform.forward * 21f, Vector3.up);
	}

	void Place(){
		placing = !placing;
	}
	//DESARROLLO DRAWING
	void Draw(){
		drawing = !drawing;
		growing = false;
	}

	void Grow(){
		if (newIvy){
			growing = !growing;
			newIvyPoints.growing = growing;
			drawing = false;
		}
		else{
			Debug.Log("Place an ivy in the scene first");
		}
	}

	void PerformanceMode(){
		performanceMode = !performanceMode;
		if (newIvy){
			newIvyGeom.performanceMode = performanceMode;
			newIvyLeaves.performanceMode = performanceMode;
		}
	}

	void Randomize(){
		newIvyParameters.randomSeed = Mathf.FloorToInt (UnityEngine.Random.Range(-10000, 10000));
		if (newIvy){
			newIvyPoints.randomSeed = newIvyParameters.randomSeed;
			newIvyLeaves.randomSeed = newIvyParameters.randomSeed;
		}
	}

	void Step(){
		if (newIvy){
			newIvyPoints.growing = true;
			if (firstIteration){
				FirstIteration();
			}
			newIvyPoints.SendMessage("Update");
			newIvyPoints.growing = false;
		}
	}

	//Método para hacer el snapshot de la malla
	void Snapshot(){
		if (newIvy){
			string path = EditorUtility.SaveFilePanelInProject("Save ivy mesh as...", "NewIvyMesh", "asset", "");
			if (path != ""){
				if (generateLightmapUvs){
					newIvyGeom.GenerateLightmapUvs();
				}
				snapShot = new GameObject();
				snapShot.name = "New Ivy Snapshot";
				snapShot.transform.position = newIvy.transform.position;
				snapShot.transform.rotation = newIvy.transform.rotation;
				MeshFilter snapShotMF = snapShot.AddComponent<MeshFilter>();
				MeshRenderer snapShotMR = snapShot.AddComponent<MeshRenderer>();

				AssetDatabase.CreateAsset( Instantiate(newIvyGeom.mesh), path );
				AssetDatabase.SaveAssets();
				Debug.Log("Ivy mesh saved at " + path);

				snapShotMF.mesh = (Mesh)AssetDatabase.LoadAssetAtPath(path, typeof(Mesh));
				MeshRenderer newIvyMR = newIvy.gameObject.GetComponent<MeshRenderer>();
				snapShotMR.sharedMaterials = newIvyMR.sharedMaterials;
			}
			else{
				Debug.Log("Snapshot canceled. Couldn't find a path");
			}
		}
	}

	//En ese método que se llama también al cerrar, se borra toda la información para que se pueda empezar de nuevo
	void CleanUp(){
		if (newIvy){			
			newIvyGeom.CleanUp();
		}
		//DESARROLLO DRAWING
		drawBranchCounter = 0;
		drawingBranch = 0;
		newIvyPoints = null;
		newIvyGeom = null;
		growing = false;
		firstIteration = true;
		UnityEngine.Object.DestroyImmediate(newIvy);
	}

	void Reset(){
		if (newIvyPoints){
			newIvyLeaves.CleanUp();
			newIvyPoints.CleanUp();
			newIvyGeom.CleanUp();
			firstIteration = true;
		}
	}

	void FirstIteration(){
		Reset();
		newIvyPoints.SendMessage("Start");
		newIvyGeom.offsetPos = newIvy.transform.position;
		newIvyGeom.offsetRot = Quaternion.Inverse(newIvy.transform.rotation);
		firstIteration = false;
	}

	void SendParameters(){
		newIvyProc.parameters = newIvyParameters;
		newIvyProc.selectedPreset = presetSelected;
	//Casuistica para la asignación de materiales, malla, y booleanos de generate mesh y branch
		if (newIvyParameters.generateBranch && newIvyParameters.generateLeaves){
			Material[] newMats = new Material[2];
			newMats[0] = newIvyParameters.branchMaterial;
			newMats[1] = newIvyParameters.leavesMaterial;
			meshRenderer.sharedMaterials = newMats;
		}
		if (newIvyParameters.generateBranch && !newIvyParameters.generateLeaves){
			Material[] newMats = new Material[1];
			newMats[0] = newIvyParameters.branchMaterial;
			meshRenderer.sharedMaterials = newMats;
		}
		if (newIvyParameters.generateLeaves && !newIvyParameters.generateBranch){
			Material[] newMats = new Material[1];
			newMats[0] = newIvyParameters.leavesMaterial;
			meshRenderer.sharedMaterials = newMats;
		}
		if (newIvyParameters.leavesMesh){
			newIvyLeaves.mesh = newIvyParameters.leavesMesh;
		}
		else{
			Material[] newMats = new Material[1];
			newMats[0] = newIvyParameters.branchMaterial;
			meshRenderer.sharedMaterials = newMats;
		}

		newIvyPoints.randomSeed = newIvyParameters.randomSeed ;
		newIvyPoints.stepSize = newIvyParameters.stepSize ;
		newIvyPoints.gravity = newIvyParameters.gravity ;
		newIvyPoints.maxBranchs = newIvyParameters.maxBranchs ;
		newIvyPoints.branchProbavility = newIvyParameters.branchProvability ;
		newIvyPoints.fallProbavilityOnCorner = newIvyParameters.fallProbavilityOnCorner ;
		newIvyPoints.directionAmplitude = newIvyParameters.directionAmplitude ;
		newIvyPoints.directionFrequency = newIvyParameters.directionFrequenzy  ;
		newIvyPoints.directionRandomness = newIvyParameters.directionRandomness  ;
		newIvyPoints.distanceToSurface = newIvyParameters.distanceToSurface  ;
		newIvyPoints.distanceToSurfaceAmplitude = newIvyParameters.distanceToSurfaceAmplitude;
		newIvyPoints.distanceToSurfaceFrequency = newIvyParameters.distanceToSurfaceFrequenzy  ;
		newIvyPoints.leaveEvery = newIvyParameters.leaveEvery;
		newIvyPoints.randomLeaveEvery = newIvyParameters.leaveEveryRandomness ;
		newIvyPoints.optimizeAngleBias = optimizeAngleBias;

		newIvyGeom.generateBranch = newIvyParameters.generateBranch ;
		newIvyGeom.sides = newIvyParameters.sides ;
		newIvyGeom.maxRadius = newIvyParameters.maxRadius ;
		newIvyGeom.minRadius = newIvyParameters.minRadius ;
		newIvyGeom.tipInfluence = newIvyParameters.tipInfluenceBranch ;
		newIvyGeom.radiusVarFrequenzy = newIvyParameters.radiusVariationFrequenzy ;
		newIvyGeom.radiusVarPhase = newIvyParameters.radiusVariationPhase ;
		newIvyGeom.uvU = newIvyParameters.uvU ;
		newIvyGeom.uvV = newIvyParameters.uvV ;

		newIvyLeaves.generateLeaves = newIvyParameters.generateLeaves ;
		newIvyLeaves.maxScale = newIvyParameters.maxScale ;
		newIvyLeaves.minScale = newIvyParameters.minScale ;
		newIvyLeaves.tipInfluence = newIvyParameters.tipInfluenceLeave ;
		newIvyLeaves.offset = newIvyParameters.positionOffset ;
		newIvyLeaves.offsetRandomness = newIvyParameters.positionOffsetRandomness ;
		newIvyLeaves.globalOrientation = newIvyParameters.globalDirection ;
		newIvyLeaves.orientation = newIvyParameters.directionVector ;
		newIvyLeaves.orientationRandomness = newIvyParameters.directionVectorRandomness ;
		newIvyLeaves.globalDirection = newIvyParameters.globalDirectionVector;
		newIvyLeaves.globalDirectionRandomness = newIvyParameters.globalDirectionVectorRandomness ;
		newIvyLeaves.globalOrientationInfluence = newIvyParameters.globalDirectionInfluence;

	}

	void NewPreset(string presetName){
		var dirInfo = new DirectoryInfo(presetsPath);
		var fileInfo = dirInfo.GetFiles();

		bool goOn = true;
		for (int i = 0; i < presets.Count; i++){
			if (presetName == Path.GetFileNameWithoutExtension(presets[i].path)){
				goOn = false;
				Debug.Log("Name already in use");
			}
			else{
				goOn = true;
			}
		}
		if (fileInfo.Length == 0){
			goOn = true;
		}
		if (presetName == null){
			Debug.Log("Please, write a name for the preset");
			goOn = false;
		}
		if (presetName == ""){
			Debug.Log("Please, write a name for the preset");
			goOn = false;
		}

		if (goOn){
			Preset newPreset = new Preset();
			newPreset.presetName = presetName;
			newPreset.presetParameters = newIvyParameters;
			newPreset.path = "Assets/3Dynamite/RealIvy/Presets/" + presetName + ".json";
			string path = newPreset.path;
			string str = JsonUtility.ToJson(newPreset);
			System.IO.File.WriteAllText(path, str);
			AssetDatabase.Refresh();
			AssetDatabase.SaveAssets();
			presets.Clear();
			LoadPresets();
		}
	}

	void SavePresets(){
		foreach (Preset p in presets){
			Preset newPreset = new Preset();
			newPreset.presetName = p.presetName;
			newPreset.presetParameters = p.presetParameters;
			newPreset.deletable = p.deletable;
			newPreset.path = "Assets/3Dynamite/RealIvy/Presets/" + p.presetName + ".json";
			string path = newPreset.path;
			string str = JsonUtility.ToJson(newPreset);
			System.IO.File.WriteAllText(path, str);
			AssetDatabase.Refresh();
			AssetDatabase.SaveAssets();
		}
	}

	void LoadPresets(){
		var dirInfo = new DirectoryInfo(presetsPath);
		var fileInfo = dirInfo.GetFiles();
		for (int i = 0; i < fileInfo.Length; i++){
			if (Path.GetExtension(fileInfo[i].FullName) == ".json"){
				string jsonString = File.ReadAllText(fileInfo[i].FullName);
				presets.Add(JsonUtility.FromJson<Preset>(jsonString));
			}
		}
		foreach (Preset preset in presets){
			preset.presetParameters.branchMaterial = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(preset.presetParameters.branchMaterialGUID)) as Material;
			preset.presetParameters.leavesMaterial = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(preset.presetParameters.leavesMaterialGUID)) as Material;
			preset.presetParameters.leavesMesh = AssetDatabase.LoadAssetAtPath<Mesh>(AssetDatabase.GUIDToAssetPath(preset.presetParameters.leavesMeshGUID)) as Mesh;
		}
	}

	void DeletePreset(int i){
		Debug.Log(presets[i].presetName +  " deleted");
		File.Delete(presets[i].path);
		File.Delete(presets[i].path + ".meta");
		AssetDatabase.Refresh ();
		presets.RemoveAt(i);
	}
}
#endif