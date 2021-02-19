using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TutorialTrigger : MonoBehaviour
{
    public int tutorialNumber;
    private void OnTriggerEnter(Collider other)
    {
        EventManager.DispatchEvent(GameEvent.TRIGGER_TUTORIAL, tutorialNumber);
    }

    private void OnTriggerExit(Collider other)
    {
        EventManager.DispatchEvent(GameEvent.TRIGGER_TUTORIAL_STOP, tutorialNumber);
    }
    
    private void OnDrawGizmos()
    {
        var collider = GetComponent<BoxCollider>();
        Gizmos.color = new Color(255, 240, 0, 0.7f); ;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(collider.center, collider.size);
    }
}
