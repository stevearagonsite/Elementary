using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryDialogueStart : MonoBehaviour
{
    public Dialogue dialogueObject;
    public bool oneTimeStory;
    public bool disablePlayer;
    public bool fadeTransition;
    public bool startBlack;
    public bool endBlack;

    private void OnTriggerEnter(Collider other)
    {
        StoryTextManager.instance.PlayDialogue(dialogueObject, fadeTransition, startBlack, endBlack);
        if (disablePlayer)
        {
            EventManager.DispatchEvent(GameEvent.STORY_START);
            Debug.Log("Start Story: " + gameObject.name);

        }
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
