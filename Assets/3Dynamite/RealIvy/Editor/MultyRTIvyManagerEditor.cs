using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MultiRTIvyManager))]
[CanEditMultipleObjects]
public class MultyRTIvyManagerEditor : Editor {

	MultiRTIvyManager multiManager;

	SerializedProperty rtIvyControllers;

	void Awake(){
		multiManager = (MultiRTIvyManager)target;

		rtIvyControllers = serializedObject.FindProperty ("rtIvyControllers");

		multiManager.RecieveData ();
	}

	public override void OnInspectorGUI () {
		GUILayoutUtility.GetRect (0f, 5f);
		EditorGUILayout.LabelField ("List of RT Ivy Controllers", EditorStyles.largeLabel);
		EditorGUILayout.Space ();

		GUILayout.BeginHorizontal ();
		if (GUILayout.Button( "Add Element", EditorStyles.miniButtonLeft)){
			multiManager.rtIvyControllers.Add(null);
			multiManager.lifeTimes.Add(2f);
			multiManager.delays.Add(0f);
		}
		if (GUILayout.Button("Find in Children", EditorStyles.miniButtonRight)){
			multiManager.FindInChildren ();
		}
		GUILayout.EndHorizontal ();


		Rect columnRect = GUILayoutUtility.GetRect (0f, 15f);
		EditorGUI.LabelField (new Rect (columnRect.x, columnRect.y, columnRect.width * 0.55f, columnRect.height), "Object");
		EditorGUI.LabelField (new Rect (columnRect.x + columnRect.width * 0.58f, columnRect.y, columnRect.width * 0.15f, columnRect.height), "LifeTime");
		EditorGUI.LabelField (new Rect (columnRect.x + columnRect.width * 0.75f, columnRect.y, columnRect.width * 0.15f, columnRect.height), "Delay");
		EditorGUI.LabelField (new Rect (columnRect.x + columnRect.width * 0.92f, columnRect.y, columnRect.width * 0.07f, columnRect.height), "Del");

		for (int i = 0; i < rtIvyControllers.arraySize; i++) {
			Rect tableRect = GUILayoutUtility.GetRect (0f, 20f);

			multiManager.rtIvyControllers [i] = EditorGUI.ObjectField (new Rect (tableRect.x, tableRect.y, tableRect.width * 0.55f, tableRect.height * 0.8f), multiManager.rtIvyControllers [i], typeof(RTIvyController), true) as RTIvyController;

			multiManager.lifeTimes[i] = EditorGUI.FloatField (new Rect (tableRect.x + tableRect.width * 0.58f, tableRect.y, tableRect.width * 0.15f, tableRect.height * 0.8f), multiManager.lifeTimes[i]);
			multiManager.delays[i] = EditorGUI.FloatField (new Rect (tableRect.x + tableRect.width * 0.75f, tableRect.y, tableRect.width * 0.15f, tableRect.height * 0.8f), multiManager.delays[i]);
			if (GUI.Button(new Rect (tableRect.x + tableRect.width * 0.92f, tableRect.y, tableRect.width * 0.07f, tableRect.height * 0.8f), "X")){
				multiManager.rtIvyControllers.RemoveAt (i);
				multiManager.lifeTimes.RemoveAt (i);
				multiManager.delays.RemoveAt (i);
				break;
			}
		}



		multiManager.SendData ();
	}
}
