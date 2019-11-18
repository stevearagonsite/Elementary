using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathingScenario : MonoBehaviour {

    MeshRenderer[] mesh;
	void Start () {
        mesh = GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < mesh.Length; i++)
        {
            LevelManager.instance.AddBreathingMaterial(mesh[i].material);
        } 
	}

}
