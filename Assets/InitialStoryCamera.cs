using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialStoryCamera : MonoBehaviour
{
    public Camera storyCam;

    void Start()
    {
        EventManager.AddEventListener(GameEvent.START_GAME, OnStartGame);
        
    }

    private void OnStartGame(object[] parameterContainer)
    {
        EventManager.DispatchEvent(GameEvent.CAMERA_STORY, storyCam);
        Destroy(this);
    }

    private void OnDestroy()
    {
        EventManager.RemoveEventListener(GameEvent.START_GAME, OnStartGame);
    }
}
