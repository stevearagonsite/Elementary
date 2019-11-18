using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ZFightFinder : EditorWindow
{
    List<GameObject> newSelection;

    [MenuItem("Window/Utilities")]
    public static void ShowWindow()
    {
        GetWindow<ZFightFinder>("Z Fight");
    }

    void OnGUI()
    {
        GUILayout.Label("Search GameObjects In the same Position", EditorStyles.boldLabel);

        if (GUILayout.Button("Search"))
        {
            SearchGameObjects();
            Selection.objects = newSelection.ToArray();
        }
    }

    void SearchGameObjects()
    {
        newSelection = new List<GameObject>();
        var totalGameObjects = Selection.gameObjects;
        Debug.Log(totalGameObjects.Length);
        for (int i = 0; i < totalGameObjects.Length; i++)
        {
            for (int j = i+1; j < totalGameObjects.Length; j++)
            {
                
                if(totalGameObjects[i].transform.position == totalGameObjects[j].transform.position)
                {
                    if(!newSelection.Contains(totalGameObjects[j]))
                    {
                        newSelection.Add(totalGameObjects[j]);
                    }  
                }
                
            }
        }
        Debug.Log(newSelection.Count);
    }

}
