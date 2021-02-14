using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioClipHandler))]
public class MusicOnStart : MonoBehaviour
{

    public float transitionTime;
    private AudioClipHandler _clipHandler;
    void Start()
    {
        _clipHandler = GetComponent<AudioClipHandler>();
        _clipHandler.PlayFadeIn(transitionTime);
    }

}
