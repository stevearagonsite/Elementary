using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class HasKeyCheck : MonoBehaviour
{
    public UnityEvent onCheckSuccesfull;
    public UnityEvent onCheckUnsuccesfull;

    private void OnTriggerEnter(Collider other)
    {
        if (GameObjectiveManager.instance.CheckEndOfLevelGoals())
        {
            if (onCheckSuccesfull != null)
                onCheckSuccesfull.Invoke();
        }
        else
        {
            if (onCheckUnsuccesfull != null)
                onCheckUnsuccesfull.Invoke();
        }
    }

    private void OnDrawGizmos()
    {
        var collider = GetComponent<BoxCollider>();
        Gizmos.color = new Color(100, 200, 0, 0.7f); ;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(collider.center, collider.size);
    }
}
