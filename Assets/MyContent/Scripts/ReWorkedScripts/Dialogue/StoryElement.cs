using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryElement : MonoBehaviour {

    public Dialogue[] dialogue;
    int dialogueNumber;


    public void LoadDialogue(object[] parameterContainer)
    {
        DialogueManager.instance.StartDialogue(dialogue[dialogueNumber], true);
        dialogueNumber ++;
    }

}
