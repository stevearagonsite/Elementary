using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMoveOnCinematic))]
public class StartCinematicStory : MonoBehaviour
{
    private PlayerMoveOnCinematic _move;

    private void Start()
    {
        _move = GetComponent<PlayerMoveOnCinematic>();
        EventManager.AddEventListener(GameEvent.TRANSITION_FADEIN_FINISH, OnStartScreen);
    }

    private void OnStartScreen(object[] pC)
    {
        EventManager.RemoveEventListener(GameEvent.TRANSITION_FADEIN_FINISH, OnStartScreen);
        StartCoroutine(StartCinematic());
    }

    private IEnumerator StartCinematic()
    {
        yield return new WaitForEndOfFrame();
        EventManager.DispatchEvent(GameEvent.STORY_START, _move.nodes);
        Debug.Log("Start Story: " + gameObject.name);
        Destroy(this);
    }

    private void OnDestroy()
    {
        EventManager.RemoveEventListener(GameEvent.TRANSITION_FADEIN_FINISH, OnStartScreen);
    }
}
