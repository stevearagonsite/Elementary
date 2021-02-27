using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionController : MonoBehaviour
{
    Animator _anim;
    private bool _stopSounds = true;
    void Start()
    {
        _anim = GetComponent<Animator>();
        EventManager.AddEventListener(GameEvent.START_LEVEL_TRANSITION, FadeToBlackTransition);
        EventManager.AddEventListener(GameEvent.TRANSITION_FADEIN_DEMO, FadeToSceneTransition);
        EventManager.AddEventListener(GameEvent.START_DEATH_TRANSITION, FadeToDeath);
    }

    private void FadeToDeath(object[] parameterContainer)
    {
        _stopSounds = false;
        FadeToBlackTransition(null);
    }

    private void FadeToBlackTransition(object[] p)
    {
        _anim.SetBool("fade",true);
    }

    public void OnFadeEnd()
    {
        EventManager.DispatchEvent(GameEvent.TRANSITION_FADEOUT_WIN_FINISH);
        if (_stopSounds)
        {
            EventManager.DispatchEvent(GameEvent.STOP_ALL_SONUNDS);
            
        }
        _anim.SetBool("fade", false);
        _stopSounds = true;
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
        EventManager.RemoveEventListener(GameEvent.START_DEATH_TRANSITION, FadeToDeath);
    }

}
