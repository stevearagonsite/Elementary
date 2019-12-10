using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ProceduralIvy))]
[CanEditMultipleObjects]
public class ProceduralIvyEditor : Editor {

	ProceduralIvy procIvy;

	GUISkin realIvyGuiskin;
	GUIStyle realIvyButtonGreen;

	void Awake(){
		realIvyGuiskin =  (GUISkin)AssetDatabase.LoadAssetAtPath("Assets/3Dynamite/RealIvy/Guiskins/RealIvyGuiskin.guiskin", typeof(GUISkin));
		realIvyButtonGreen = new GUIStyle(realIvyGuiskin.GetStyle("buttongreen"));
		procIvy = (ProceduralIvy)target;
	}

	public override void OnInspectorGUI () {

		GUILayoutUtility.GetRect (0f, 15f);
		Rect buttonRect = GUILayoutUtility.GetRect (0f, 50f);

		if (GUI.Button(buttonRect, "Convert to Runtime", realIvyButtonGreen)){
			procIvy.ConvertToRuntime ();
		}

		GUILayoutUtility.GetRect (0f, 10f);
	}
}