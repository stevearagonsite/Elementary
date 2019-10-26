using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorEventsBehaviour : MonoBehaviour {

    private Animator _anim;
    public bool landEnd;

	void Start () {
        landEnd = true;
        _anim = GetComponent<Animator>();

	}

    public void LandEnd()
    {
        landEnd = true;
        _anim.SetBool("toLand", false);
    }

    public void LandStart()
    {
        landEnd = false;
    }

}
