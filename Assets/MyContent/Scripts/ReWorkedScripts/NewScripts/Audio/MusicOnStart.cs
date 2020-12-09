using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicOnStart : MonoBehaviour
{
    public AudioClip clip;
    [Range(0, 1)]
    public float volume;
    public float transitionTime;

    void Start()
    {
        AudioManager.instance.PlayMusicWithFade(clip, transitionTime, volume);
    }

}
