using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioClipHandler))]
public class SoundOnCollisionEnter : MonoBehaviour
{
    private AudioClipHandler _clipHandler;
    private void OnCollisionEnter(Collision other)
    {
        if (_clipHandler == null)
        {
            _clipHandler = GetComponent<AudioClipHandler>();
        }
        _clipHandler.Play();
        Debug.Log("Play sonido: " + other.gameObject.name);
    }
}
