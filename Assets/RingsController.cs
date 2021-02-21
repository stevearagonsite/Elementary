using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingsController : MonoBehaviour
{
    public float _alphaTransitionTime;
    public float _keyAmountTransitionTime;
    private Animator _anim;
    private Renderer _rend;

    private void Start()
    {
        _anim = GetComponent<Animator>();
        _rend = GetComponent<Renderer>();
        EventManager.AddEventListener(GameEvent.SPAWN_RINGS, OnActivate);
    }
    public void OnActivate(object[]p)
    {
        EventManager.RemoveEventListener(GameEvent.SPAWN_RINGS, OnActivate);
        StartCoroutine(Activate());
    }

    public void OnDeactivate(object[] p)
    {
        StartCoroutine(Deactivate());
    }

    private IEnumerator Activate()
    {
        var tick = 0f;
        while (tick < _alphaTransitionTime)
        {
            tick += Time.deltaTime;
            _rend.material.SetFloat("_alpha", tick / _alphaTransitionTime);
            yield return null;
        }
        _rend.material.SetFloat("_alpha", 1);
        
        tick = 0;
        while (tick < _keyAmountTransitionTime)
        {
            tick += Time.deltaTime;
            _rend.material.SetFloat("_keyAmount", 1 - (tick / _alphaTransitionTime));
            yield return null;
        }

        _rend.material.SetFloat("_keyAmount", 0);

    }

    private IEnumerator Deactivate()
    {
        var tick = 0f;

        while (tick < _keyAmountTransitionTime)
        {
            tick += Time.deltaTime;
            _rend.material.SetFloat("_keyAmount", tick / _alphaTransitionTime);
            yield return null;
        }
        _rend.material.SetFloat("_keyAmount", 1);
        tick = 0;
        while (tick < _alphaTransitionTime)
        {
            tick += Time.deltaTime;
            _rend.material.SetFloat("_alpha", 1 - (tick / _alphaTransitionTime));
            yield return null;
        }
        _rend.material.SetFloat("_alpha", 0);
    }
}
