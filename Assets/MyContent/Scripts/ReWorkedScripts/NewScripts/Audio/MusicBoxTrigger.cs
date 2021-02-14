using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioClipHandler))]
public class MusicBoxTrigger : MonoBehaviour
{
    public float transitionTime = 1;

    private AudioClipHandler _clipHandler;
    private void OnTriggerEnter(Collider other)
    {
        if (_clipHandler == null)
        {
            _clipHandler = GetComponent<AudioClipHandler>();
        }
        _clipHandler.PlayFadeIn(transitionTime);
        Debug.Log("Play Music: " + other.name);
    }

    private void OnTriggerExit(Collider other)
    {
        if (_clipHandler == null)
        {
            _clipHandler = GetComponent<AudioClipHandler>();
        }
        _clipHandler.StopFadeOut(transitionTime);
    }
}
