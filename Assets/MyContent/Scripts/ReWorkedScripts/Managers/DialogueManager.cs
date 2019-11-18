using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {

    static DialogueManager _instance;
    public static DialogueManager instance { get { return _instance; } }

    public Text nameText;
    public Text dialogueText;
    public Image picture;
    public Animator dialogueBoxAnimator;
    public Animator pictureBoxAnimator;

    Queue<string> _sentences;

    bool isPaused = false;
    public bool closeBoxes;

    void Awake()
    {
        if (instance == null) _instance = this;
        _sentences = new Queue<string>();
    }
    
    void Start () {
        
        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
	}

    private void Execute()
    {
        if (GameInput.instance.initialJumpButton)
        {
            DisplayNextSentence();
        }
    }

    public void StartDialogue(Dialogue dialogue, bool picture)
    {
        closeBoxes = dialogue.last;
        dialogueBoxAnimator.SetBool("IsOpen", true);
        if (picture)
        {
            pictureBoxAnimator.SetBool("IsOpen", true);
            this.picture.sprite = dialogue.picture;
        }
        nameText.text = dialogue.name;
        
        _sentences.Clear();

        foreach (var sentence in dialogue.sentences)
        {
            _sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if(_sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        var sentence =_sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));

    }

    IEnumerator TypeSentence (string sentence)
    {
        dialogueText.text = "";
        if (!isPaused)
        {

            foreach (var letter in sentence.ToCharArray())
            {
                dialogueText.text += letter;
                yield return null;
            }
        }
    }

    void EndDialogue()
    {
        if (closeBoxes)
        {
            dialogueBoxAnimator.SetBool("IsOpen", false);
            pictureBoxAnimator.SetBool("IsOpen", false);
            EventManager.DispatchEvent(GameEvent.STORY_END);
        }
        else
        {
            EventManager.DispatchEvent(GameEvent.STORY_NEXT);
        }
    }

    private void OnDestroy()
    {
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
    }
}
