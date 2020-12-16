using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayAnimation : MonoBehaviour {

    private Animator _animator;
    public bool initalPlay = false;

    void OnEnable() {
        _animator = GetComponent<Animator>();
        _animator.SetFloat("speed", (initalPlay ? 1 : 0));
    }

    public void toggleAnim(bool b)
    {
        Debug.Log($"ANIM {b}");
        if (b)
        {
            _animator.SetFloat("speed", 1f);
        }
        else
        {
            _animator.SetFloat("speed", -1f);
        }
    }

    public void StopAnimation(float dir)
    {
        if(_animator.GetFloat("speed") == dir)
            _animator.SetFloat("speed", 0);
    }
}
