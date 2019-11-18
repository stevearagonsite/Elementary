using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelZeroCutScene : MonoBehaviour, CutScene
{

    Camera _cam;

    public Transform[] cameraPositions;
    [SerializeField]
    int storyCount;

    void Start()
    {
        _cam = GetComponent<Camera>();
        //_cam.enabled = false;
    }

    public void Enter()
    {
        _cam.enabled = true;
        EventManager.AddEventListener(GameEvent.STORY_NEXT, NextPosition);
    }

    public void Execute()
    {
        transform.position = cameraPositions[storyCount].position;
        transform.rotation = cameraPositions[storyCount].rotation;
    }

    public void Exit()
    {
        _cam.enabled = false;
        EventManager.RemoveEventListener(GameEvent.STORY_NEXT, NextPosition);
    }

    private void NextPosition(object[] parameterContainer)
    {
        storyCount++;
    }

    private void OnDestroy()
    {
        EventManager.RemoveEventListener(GameEvent.STORY_NEXT, NextPosition);
    }
}
