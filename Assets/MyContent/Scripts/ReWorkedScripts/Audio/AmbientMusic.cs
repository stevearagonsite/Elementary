using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientMusic : MonoBehaviour {

    public string BGMName;
	void Start () {
        AudioManager.instance.Play(BGMName);
	}
	
}
