using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFixTrigger : MonoBehaviour {

    public float distance;
    public float targetX;
    public float targetY;

    void OnTriggerEnter(Collider c)
    {
        if(c.gameObject.layer == 9)
        {
            EventManager.DispatchEvent(GameEvent.CAMERA_FIXPOS, targetX, targetY ,distance);
        }
    }

    private void OnDrawGizmos()
    {
        var collider = GetComponent<BoxCollider>();
        Gizmos.color = new Color(0, 10, 70, 0.7f); ;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(collider.center, collider.size);
    }
}
