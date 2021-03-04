using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioClipHandler))]
public class SoundOnCollisionEnter : MonoBehaviour
{
    private AudioClipHandler _clipHandler;
    private AudioSource _source;
    private void Awake()
    {
        _source = GetComponent<AudioSource>();
        _source.mute = true;
        Invoke("UnMute", 1);
    }

    private void UnMute()
    {
        _source.mute = false;
    }
    private void OnCollisionEnter(Collision other)
    {
        if (_clipHandler == null)
        {
            _clipHandler = GetComponent<AudioClipHandler>();
        }
        _clipHandler.Play();
    }
}
