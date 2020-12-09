using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class SoundOnTriggerEnter : MonoBehaviour
{
    public AudioClip clip;
    public float volume;
    private void OnTriggerEnter(Collider other)
    {
        AudioManager.instance.PlaySFX(clip, volume);
    }
}
