using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class MusicBoxTrigger : MonoBehaviour
{
    public AudioClip clip;
    [Range(0,1)]
    public float volume;
    public float transitionTime;

    private void OnTriggerEnter(Collider other)
    {
        AudioManager.instance.PlayMusicWithFade(clip, transitionTime, volume);
    }
}
