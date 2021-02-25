using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMusicVolumeOnTrigger : MonoBehaviour
{
    public AudioClipHandler audioClipHandler;
    public float targetVolume;
    private void OnTriggerEnter(Collider other)
    {
        audioClipHandler.FadeToVolume(targetVolume);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        var collider = GetComponent<BoxCollider>();
        Gizmos.color = new Color(0, 0, 0.7f, 0.7f); ;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(collider.center, collider.size);
    }
#endif
}
