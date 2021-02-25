using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryDialogueCheckGoals : MonoBehaviour
{
    public Dialogue dialogueObject;

    public void StartNoKeyDialogue()
    {
        StoryTextManager.instance.PlayDialogue(dialogueObject);
    }
    public void DisablePlayer()
    {
        Debug.Log("Start Story: " + gameObject.name);
        EventManager.DispatchEvent(GameEvent.STORY_START);
    }
}
