using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class StoryTextManager : MonoBehaviour
{
    private static StoryTextManager _instance;
    public static StoryTextManager instance { get { return _instance; } }

    public TextMeshProUGUI textField;
    public Animator canvasAnimator;
    public Animator textAnimator;
    public Canvas skipCanvas;
    public Image fadeTransitionImage;

    private bool _isPlaying;
    private float _textDuration;
    private BaseNode _actualNode;
    private float _tick;
    private int _dialogueIndex = 0;
    private Dialogue _dialogue;

    private bool _endBlack;

    private void Awake()
    {
        if (instance == null) _instance = this;
    }
    void Start()
    {
        canvasAnimator.Play("idle");
        textAnimator.Play("idle");
    }

    public void PlayDialogue(Dialogue dialogue , bool fadeTransition = false, bool startBlack = false, bool endBlack = false)
    {
        _dialogue = dialogue;
        _dialogueIndex = 0;
        fadeTransitionImage.enabled = fadeTransition;
        _endBlack = endBlack;
        var startAnimation = startBlack ? "Start Black" : "Start";
        if (!_isPlaying)
        {
            _isPlaying = true;
            textAnimator.Play(startAnimation);
            canvasAnimator.Play(startAnimation);
            ShowDialogueSentence(_dialogue.nodes[_dialogueIndex]);
        }
    }

    private void ShowDialogueSentence(BaseNode baseNode)
    {
        _dialogueIndex++;
        if (_dialogueIndex < _dialogue.nodes.Count)
            _actualNode = _dialogue.nodes[_dialogueIndex];
        else
            _actualNode = null;
        if (baseNode.duration > 0)
        {
            _textDuration = baseNode.duration;
            UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
        }
        if (baseNode.skipable)
        {
            InputManager.instance.AddAction(InputType.Skip_Dialogue, OnNextStory);
            skipCanvas.enabled = true;
        }
        else
        {
            skipCanvas.enabled = false;
        }
        textField.text = baseNode.dialog;
    }

    private void OnNextStory()
    {
        InputManager.instance.RemoveAction(InputType.Skip_Dialogue, OnNextStory);
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
        GoToNext();
        _tick = 0;
    }

    private void Execute()
    {
        if (_tick < _textDuration)
        {
            _tick += Time.deltaTime;
        }
        else
        {
            _tick = 0;
            UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
            InputManager.instance.RemoveAction(InputType.Skip_Dialogue, OnNextStory);
            GoToNext();
        }
    }

    private void GoToNext()
    {
        if (_actualNode != null)
        {
            textAnimator.Play("Next");
            ShowDialogueSentence(_actualNode);
            EventManager.DispatchEvent(GameEvent.STORY_NEXT);
        }
        else
        {
            var endAnimation = _endBlack ? "Exit Black" : "Exit";
            textAnimator.Play(endAnimation);
            canvasAnimator.Play(endAnimation);
            EventManager.DispatchEvent(GameEvent.STORY_END);
            EventManager.DispatchEvent(GameEvent.CAMERA_NORMAL);
            skipCanvas.enabled = false;
            _isPlaying = false;
        }
    }
}
