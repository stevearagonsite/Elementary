using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraNormalTrigger : MonoBehaviour {

    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.layer == 9)
        {
            EventManager.DispatchEvent(GameEvent.CAMERA_NORMAL);
        }
    }

    private void OnDrawGizmos()
    {
        var collider = GetComponent<BoxCollider>();
        Gizmos.color = new Color(100, 200, 200, 0.7f); ;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(collider.center, collider.size);
    }
}
