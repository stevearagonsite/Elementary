using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioClipHandler))]
public class SoundOnTriggerEnter : MonoBehaviour
{
    private AudioClipHandler _clipHandler;
    private void OnTriggerEnter(Collider other)
    {
        if (_clipHandler == null)
        {
            _clipHandler = GetComponent<AudioClipHandler>();
        }
        _clipHandler.Play();
        Debug.Log("Play sonido: " + other.name);
    }
}
