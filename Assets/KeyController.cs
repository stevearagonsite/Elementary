using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyController : MonoBehaviour
{
    private Animator _anim;
    private Renderer _rend;
    private float _transitionTime = 1f;
    public Light keyLight;

    void Start()
    {
        _anim = GetComponent<Animator>();
        _rend = GetComponent<Renderer>();
        EventManager.AddEventListener(GameEvent.GET_KEY_EVENT, OnGetKey);
    }

    private void OnGetKey(object[] parameterContainer)
    {
        EventManager.RemoveEventListener(GameEvent.GET_KEY_EVENT, OnGetKey);
        _anim.SetTrigger("GetKey");
    }

    public void KeyInHand()
    {
        StartCoroutine(DisapearKey());
    }

    private IEnumerator DisapearKey()
    {
        var tick = 0f;
        while (tick < _transitionTime)
        {
            _rend.materials[1].SetFloat("_keyAmount", tick / _transitionTime);
            tick += Time.deltaTime;
            yield return null;
        }
        tick = 0;
        while (tick < _transitionTime)
        {
            for (int i = 0; i < _rend.materials.Length; i++)
            {
                _rend.materials[i].SetFloat("_alpha", 1 - (tick / _transitionTime));
            }
            keyLight.intensity = 1 - (tick / _transitionTime);
            tick += Time.deltaTime;
            yield return null;
        }

        Destroy(this);
        _rend.enabled = false;
    }

    private void OnDestroy()
    {
        EventManager.RemoveEventListener(GameEvent.GET_KEY_EVENT, OnGetKey);
    }
}
