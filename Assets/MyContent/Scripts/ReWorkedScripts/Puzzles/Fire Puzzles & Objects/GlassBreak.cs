using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassBreak : MonoBehaviour {

    FireSwitch _fSwitch;
    Animator _anim;

    public GameObject completeGlass;
    public GameObject[] glassPieces;

	// Use this for initialization
	void Start () {
        _fSwitch = GetComponentInChildren<FireSwitch>();
        _anim = GetComponent<Animator>();
        completeGlass.SetActive(true);
        for (int i = 0; i < glassPieces.Length; i++) 
        {
            glassPieces[i].SetActive(false);
        }
        _anim.enabled = false;

        _fSwitch.AddOnSwitchEvent(IsBurned);
    }
	
    void IsBurned() 
    {
        completeGlass.SetActive(false);
        for (int i = 0; i < glassPieces.Length; i++) {
            glassPieces[i].SetActive(true);
        }
        _anim.enabled = true;
    }
	
}
