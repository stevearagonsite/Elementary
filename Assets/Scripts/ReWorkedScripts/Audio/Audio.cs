using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Audio
{
    public string name;
    public AudioClip clip;

    [HideInInspector]
    public AudioSource source;

    public Transform parent;

    [Range(0,1)]
    public float volume;

    [Range(0.1f,3)]
    public float pitch;

    public bool loop;
}
