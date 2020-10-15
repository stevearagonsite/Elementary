using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObstacleChecker:MonoBehaviour
{
    public Transform col;
    public Vector3 halfside;
    public LayerMask layerMask;

    public bool CheckObstacle()
    {
        var colliders = Physics.OverlapBox(col.position, halfside, Quaternion.identity, layerMask);
        return colliders.Length > 0;
    }

    private void OnDrawGizmos()
    {
        if(col)
        {
            Gizmos.DrawWireCube(col.position, halfside * 2);
        }
    }
}
