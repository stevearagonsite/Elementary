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
        if (b)
        {
            _animator.SetFloat("speed", 1f);
        }
        else
        {
            _animator.SetFloat("speed", -1f);
        }
    }
}
