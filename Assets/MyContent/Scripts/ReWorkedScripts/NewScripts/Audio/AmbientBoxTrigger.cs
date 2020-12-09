using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class AmbientBoxTrigger : MonoBehaviour
{
    public AudioClip ambientSound;
    [Range(0, 1)]
    public float volume = 1;
    public float transitionTime = 1;
    private void OnTriggerEnter(Collider other)
    {
        AudioManager.instance.PlayAmbientWithCrossFade(ambientSound, transitionTime, volume);
    }
}
