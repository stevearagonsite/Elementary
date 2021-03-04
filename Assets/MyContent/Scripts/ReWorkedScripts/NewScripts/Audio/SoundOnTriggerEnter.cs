using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioClipHandler))]
public class SoundOnTriggerEnter : MonoBehaviour
{
    private AudioClipHandler _clipHandler;
    private void OnTriggerEnter(Collider other)
    {
        if (_clipHandler == null && other.gameObject.layer == 9)
        {
            _clipHandler = GetComponent<AudioClipHandler>();
            _clipHandler.Play();
        }
       
    }
}
