using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryDialogueStart : MonoBehaviour
{
    public Dialogue dialogueObject;
    private void OnTriggerEnter(Collider other)
    {
        StoryTextManager.instance.PlayDialogue(dialogueObject);
    }
}
