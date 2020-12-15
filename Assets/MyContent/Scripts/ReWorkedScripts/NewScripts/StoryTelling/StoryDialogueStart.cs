using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryDialogueStart : MonoBehaviour
{
    public Dialogue dialogueObject;
    public bool oneTimeStory;
    private void OnTriggerEnter(Collider other)
    {
        StoryTextManager.instance.PlayDialogue(dialogueObject);
        if (oneTimeStory)
            Destroy(this);
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        var collider = GetComponent<BoxCollider>();
        Gizmos.color = new Color(100, 100, 100, 0.7f); ;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(collider.center, collider.size);
    }
#endif
}
