using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;


[CustomEditor(typeof(IvyCaster))]
[CanEditMultipleObjects]
public class IvyCasterEditor : Editor {

	string presetsPath = "Assets/3Dynamite/RealIvy/Presets";

	List<Preset> presets = new List<Preset>();

	IvyCaster ivyCaster;

	GUISkin realIvyGuiskin;
	GUIStyle realIvyButton;
	GUIStyle realIvyButtonGreen;

	SerializedProperty preWarmEnabled;
	GUIContent contentpreWarmEnabled;
	SerializedProperty preWarm;
	GUIContent contentpreWarm;
	SerializedProperty IvyEnabled;
	SerializedProperty speed;
	GUIContent contentSpeed;
	SerializedProperty lifeTime;
	GUIContent contentLifeTime;
	SerializedProperty delay;
	GUIContent contentDelay;
	SerializedProperty speedOverLifeTimeEnabled;
	GUIContent contentSpeedOverLifeTimeEnabled;
	SerializedProperty speedOverLifeTime;
	GUIContent contentSpeedOverLifeTime;
	SerializedProperty autoOptimize;
	GUIContent contentAutoOptimize;
	SerializedProperty autoOptimizeAngle;
	GUIContent contentAutoOptimizeAngle;
	SerializedProperty generateBranches;
	GUIContent contentGenerateBranches;
	SerializedProperty generateLeaves;
	GUIContent contentGenerateLeaves;

	Texture2D bgTex;



	void Awake () {
		ivyCaster = (IvyCaster)target;
		bgTex = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3Dynamite/RealIvy/SampleAssets/Textures/Ivy_BG.png", typeof(Texture2D));
		realIvyGuiskin =  (GUISkin)AssetDatabase.LoadAssetAtPath("Assets/3Dynamite/RealIvy/Guiskins/RealIvyGuiskin.guiskin", typeof(GUISkin));
		realIvyButton = new GUIStyle(realIvyGuiskin.GetStyle("button"));
		realIvyButtonGreen = new GUIStyle(realIvyGuiskin.GetStyle("buttongreen"));
		LoadPresets ();

		AssignLabel (ivyCaster.gameObject);

		preWarmEnabled =  serializedObject.FindProperty ("preWarmEnabled");
		preWarm = serializedObject.FindProperty ("preWarm");
		speed = serializedObject.FindProperty ("maxSpeed");
		IvyEnabled = serializedObject.FindProperty ("IvyEnabled");
		lifeTime = serializedObject.FindProperty ("lifeTime");
		delay = serializedObject.FindProperty ("delay");
		speedOverLifeTimeEnabled = serializedObject.FindProperty ("speedOverLifeTimeEnabled");
		speedOverLifeTime = serializedObject.FindProperty ("speedOverLifeTime");
		autoOptimize = serializedObject.FindProperty ("autoOptimize");
		autoOptimizeAngle = serializedObject.FindProperty ("angleBias");
		generateBranches = serializedObject.FindProperty ("generateBranches");
		generateLeaves = serializedObject.FindProperty ("generateLeaves");

		contentpreWarmEnabled = new GUIContent("Prewarm", "If enabled the Ivy will grow N iterations at StartUp");
		contentpreWarm = new GUIContent ("Prewarm Iterations", "The number of iterations that the Ivy will grow at StartUp");
		contentLifeTime = new GUIContent ("Lifetime", "The time that the Ivy will grow in seconds");
		contentDelay = new GUIContent ("Delay", "The time that the Ivy will wait until start the growth");
		contentSpeed = new GUIContent ("Speed", "The maximum speed of this Ivy");
		contentSpeedOverLifeTimeEnabled = new GUIContent ("Speed Over Lifetime", "Use this for animate the speed over the lifetime with a curve");
		contentSpeedOverLifeTime = new GUIContent ("Curve");
		contentAutoOptimize = new GUIContent ("Auto Optimization", "Whether the Ivy shall or not optimize its own geometry");
		contentAutoOptimizeAngle = new GUIContent ("Angle Bias", "The minimum angle that the Auto Optimization will respect");
		contentGenerateBranches = new GUIContent ("Generate Branches", "Whether the Ivy shall or not generate branches geometry");
		contentGenerateLeaves = new GUIContent ("Generate Leaves", "Whether the Ivy shall or not generate leaves geometry");

		if (ivyCaster.firstPass) {
			Debug.Log (ivyCaster.firstPass);
			ivyCaster.firstPass = false;
			ivyCaster.selectedPreset = 0;
			ivyCaster.GetParameters (presets [0]);
		}
	}

	public static void AssignLabel(GameObject g)
	{
		Texture2D tex = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/3Dynamite/RealIvy/SampleAssets/Textures/Leave_Cast_Icon.png", typeof(Texture2D));
		Type editorGUIUtilityType  = typeof(EditorGUIUtility);
		BindingFlags bindingFlags = BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic;
		object[] args = new object[] {g, tex};
		editorGUIUtilityType.InvokeMember("SetIconForObject", bindingFlags, null, null, args);
	}

	public override void OnInspectorGUI () {
		GUILayoutUtility.GetRect (0f, 5f);
		Rect labelRect = GUILayoutUtility.GetRect (0f, 20f);

		EditorGUILayout.Space ();
		Rect presetBox = GUILayoutUtility.GetRect (0f, 150f);

		GUI.BeginGroup(new Rect(presetBox.x - 15f, presetBox.y - 25f, presetBox.width + 15f, presetBox.height + 500f));
		GUI.DrawTexture (new Rect (0, 0, bgTex.width, bgTex.height), bgTex);
		GUI.EndGroup();

		GUI.Label (labelRect, "Choose a preset to derive from in casted ivies:", EditorStyles.largeLabel);
		presetBox = new Rect (presetBox.x + presetBox.width * 0.1f, presetBox.y, presetBox.width * 0.8f, presetBox.height);
		GUI.Box(new Rect(presetBox.x, presetBox.y - 3f, presetBox.width - 15f, presetBox.height + 3f), "", EditorStyles.helpBox);

		GUI.BeginGroup (presetBox);

		ivyCaster.presetBoxScroll = GUI.BeginScrollView(new Rect(-3f, 0f, presetBox.width, presetBox.height - 3f), ivyCaster.presetBoxScroll, new Rect(0f, 0f, presetBox.width - 15f, 30f * presets.Count));

		for (int i = 0; i < presets.Count; i++) {
			GUIStyle style;
			if (i == ivyCaster.selectedPreset) {
				style = realIvyButtonGreen;
			}
			else {
				style = realIvyButton;
			}
			if (GUI.Button(new Rect( 40f, 30f * i, presetBox.width - 85f, 25f), presets[i].presetName, style)){
				ivyCaster.selectedPreset = i;
				ivyCaster.GetParameters (presets [i]);
				break;
			}
		}

		GUI.EndScrollView ();
		GUI.EndGroup ();

		Rect warningRect = GUILayoutUtility.GetRect (0f, 50f);
		warningRect = new Rect (warningRect.x + warningRect.width * 0.1f, warningRect.y + 2f, warningRect.width * 0.8f, warningRect.height * 0.9f);
		EditorGUI.HelpBox (warningRect, "You cannot change the preset during runtime after build the application. Create more than one Ivy Caster for create different kind of" +
		" ivies", MessageType.Warning);

		EditorGUILayout.Space ();

		serializedObject.Update ();

		EditorGUILayout.PropertyField (preWarmEnabled, contentpreWarmEnabled);
		if (preWarmEnabled.boolValue) {
			EditorGUILayout.PropertyField (preWarm, contentpreWarm);
		} 

		EditorGUILayout.Space ();
		EditorGUILayout.PropertyField (IvyEnabled);
		EditorGUILayout.PropertyField (lifeTime, contentLifeTime);
		EditorGUILayout.PropertyField (delay, contentDelay);
		EditorGUILayout.Space ();
		EditorGUILayout.PropertyField (speed, contentSpeed);
		EditorGUILayout.PropertyField (speedOverLifeTimeEnabled, contentSpeedOverLifeTimeEnabled);
		if (speedOverLifeTimeEnabled.boolValue) {
			EditorGUI.indentLevel += 1;
			EditorGUILayout.PropertyField (speedOverLifeTime, contentSpeedOverLifeTime);
			EditorGUI.indentLevel -= 1;
		}

		EditorGUILayout.Space ();
		EditorGUILayout.PropertyField (autoOptimize, contentAutoOptimize);

		if (autoOptimize.boolValue) {
			EditorGUI.indentLevel += 1;
			EditorGUILayout.PropertyField (autoOptimizeAngle, contentAutoOptimizeAngle);
			EditorGUI.indentLevel -= 1;
		}
		EditorGUILayout.Space ();


		EditorGUI.BeginChangeCheck ();
		EditorGUILayout.PropertyField (generateBranches, contentGenerateBranches);
		if (EditorGUI.EndChangeCheck ()) {
			if (generateBranches.boolValue && generateLeaves.boolValue) {
				Debug.LogWarning ("Check if your materials array still consistent after switching Generate Branches or Generate Leaves values");
			}
		}

		Rect generateBranchesRect = GUILayoutUtility.GetLastRect ();
		generateBranchesRect.x = generateBranchesRect.x + generateBranchesRect.width * 0.45f;
		generateBranchesRect.y = generateBranchesRect.y + generateBranchesRect.height * 0.5f;
		generateBranchesRect.width = generateBranchesRect.width * 0.55f;
		generateBranchesRect.height *= 1f;
		EditorGUI.HelpBox (generateBranchesRect, "Switching these values you could mess up the materials array", MessageType.None);

		EditorGUI.BeginChangeCheck ();
		EditorGUILayout.PropertyField (generateLeaves, contentGenerateLeaves);
		if (EditorGUI.EndChangeCheck ()) {
			if (generateBranches.boolValue && generateLeaves.boolValue) {
				Debug.LogWarning ("Check if your materials array still consistent after switching Generate Branches or Generate Leaves values");
			}
		}

		serializedObject.ApplyModifiedProperties ();
	}

	void LoadPresets(){
		presets.Clear ();
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
}