using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StumpCutScene : MonoBehaviour, CutScene {

    Camera _cam;

    public Camera mainCam;

    public Animator blackOutAnimator;

    Animator _anim;

    public StoryElement story;
    Vector3 _initialForward;
    Vector3 _initialPos;

    Vector3 _mainCamInitialForward;
    Vector3 _mainCamInitialPos;

    void Start()
    {
        _cam = GetComponent<Camera>();
        _cam.enabled = false;
        _anim = GetComponent<Animator>();
        _anim.enabled = false;
        _initialPos = transform.position;
        _initialForward = transform.forward;
    }

    public void Enter()
    {
        _cam.enabled = true;
        EventManager.AddEventListener(GameEvent.TRANSITION_FADEOUT_DEMO, FadeOutEnd);
        EventManager.AddEventListener(GameEvent.TRANSITION_FADEIN_DEMO, FadeInEnd);
        EventManager.AddEventListener(GameEvent.STORY_END, BackToGame);
        blackOutAnimator.SetTrigger("FadeOutAndIn");

        _mainCamInitialPos = mainCam.transform.position;
        _mainCamInitialForward = mainCam.transform.forward;
        _cam.transform.position = mainCam.transform.position;
        _cam.transform.forward = mainCam.transform.forward;

    }

    public void Execute()
    {

    }

    public void Exit()
    {
        _cam.enabled = false;
    }

    private void FadeOutEnd(object[] parameterContainer)
    {
        EventManager.RemoveEventListener(GameEvent.TRANSITION_FADEOUT_DEMO, FadeOutEnd);
        _cam.transform.forward = _initialForward;
        _cam.transform.position = _initialPos;
    }


    private void FadeInEnd(object[] parameterContainer)
    {

        EventManager.RemoveEventListener(GameEvent.TRANSITION_FADEIN_DEMO, FadeInEnd);
        _anim.enabled = true;
        _anim.SetTrigger("start");
        story.LoadDialogue(null);
    }

    private void BackToGame(object[] parameterContainer)
    {
        EventManager.AddEventListener(GameEvent.TRANSITION_FADEOUT_DEMO, FadeOutEndCutScene);
        blackOutAnimator.SetTrigger("FadeOutAndIn");
    }

    private void FadeOutEndCutScene(object[] parameterContainer)
    {
        EventManager.RemoveEventListener(GameEvent.TRANSITION_FADEOUT_DEMO, FadeOutEndCutScene);
        _cam.transform.forward = _initialForward;
        _cam.transform.position = _initialPos;
        
        EventManager.DispatchEvent(GameEvent.CAMERA_NORMAL);
        EventManager.RemoveEventListener(GameEvent.STORY_END, BackToGame);
    }
}
