using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientOnStart : MonoBehaviour {

    public AudioClip ambientSound;
	[Range(0,1)]
	public float volume = 1;

	public float transitionTime;

	void Start () {
		AudioManager.instance.PlayAmbientWithFade(ambientSound, transitionTime, volume);
	}
	
}
