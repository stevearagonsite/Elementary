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
    public bool stopOnExit;
    public bool destroy;
    private void OnTriggerEnter(Collider other)
    {
        if (_clipHandler == null)
        {
            _clipHandler = GetComponent<AudioClipHandler>();
        }
        _clipHandler.PlayFadeIn(transitionTime);
        Debug.Log("Play Music: " + other.name);
        if (destroy)
            Destroy(this);
    }

    private void OnTriggerExit(Collider other)
    {
        if (stopOnExit)
        {
            if (_clipHandler == null)
            {
                _clipHandler = GetComponent<AudioClipHandler>();
            }
            _clipHandler.StopFadeOut(transitionTime);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        var collider = GetComponent<BoxCollider>();
        Gizmos.color = Gizmos.color = new Color(0, 0, 0.7f, 0.7f);
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(collider.center, collider.size);
    }
#endif
}
