using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveEndOfLevelKey : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GameObjectiveManager.instance.ActivateKeyHold();
    }

    private void OnDrawGizmos()
    {
        var collider = GetComponent<BoxCollider>();
        Gizmos.color = new Color(100, 0, 0, 0.7f); ;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(collider.center, collider.size);
    }
}
