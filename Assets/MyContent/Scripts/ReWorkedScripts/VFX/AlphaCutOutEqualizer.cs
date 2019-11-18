using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlphaCutOutEqualizer : MonoBehaviour {

    Material mat;
	void Start () {
        mat = GetComponent<MeshRenderer>().material;
	}
	
	// Update is called once per frame
	void Update () {
        var alpha = mat.GetFloat("_Cutoff");
        mat.SetFloat("_Mask", alpha);
	}
}
