using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionController : MonoBehaviour
{
    Animator _anim;

    void Start()
    {
        _anim = GetComponent<Animator>();
        EventManager.AddEventListener(GameEvent.START_LEVEL_TRANSITION, FadeToBlackTransition);
        EventManager.AddEventListener(GameEvent.TRANSITION_FADEIN_DEMO, FadeToSceneTransition);
    }

    private void FadeToBlackTransition(object[] p)
    {
        _anim.SetBool("fade",true);
    }

    public void OnFadeEnd()
    {
        EventManager.DispatchEvent(GameEvent.TRANSITION_FADEOUT_WIN_FINISH);
        _anim.SetBool("fade", false);
    }

    public void OnFadeToSceneEnd()
    {
        _anim.SetBool("in",false);
        EventManager.DispatchEvent(GameEvent.TRANSITION_FADEIN_FINISH);
    }

    private void FadeToSceneTransition(object[] p)
    {
        _anim.SetBool("in",true);
        StartCoroutine(FadeInTimeOut());

    }

    private IEnumerator FadeInTimeOut()
    {
        yield return new WaitForSeconds(1.5f);
        _anim.SetBool("in", false);
    }

    private void OnDestroy()
    {
        EventManager.RemoveEventListener(GameEvent.START_LEVEL_TRANSITION, FadeToBlackTransition);
        EventManager.RemoveEventListener(GameEvent.TRANSITION_FADEIN_DEMO, FadeToSceneTransition);

    }

}
