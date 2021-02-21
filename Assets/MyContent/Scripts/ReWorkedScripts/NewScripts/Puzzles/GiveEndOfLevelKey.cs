using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveEndOfLevelKey : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GameObjectiveManager.instance.ActivateKeyHold();
        EventManager.DispatchEvent(GameEvent.KEY_TAKE);
    }

    private void OnDrawGizmos()
    {
        var collider = GetComponent<BoxCollider>();
        Gizmos.color = new Color(0, 0, 1, 0.5f); ;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(collider.center, collider.size);
    }
}
