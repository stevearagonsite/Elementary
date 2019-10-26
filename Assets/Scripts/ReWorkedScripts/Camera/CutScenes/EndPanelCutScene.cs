using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPanelCutScene : MonoBehaviour, CutScene
{
    Camera _cam;
    [Header("Camera positions")]
    public Transform[] cameraPositions;
    int cameraPositionCount = 1;

    void Start()
    {
        _cam = GetComponent<Camera>();
        _cam.enabled = false;
        EventManager.AddEventListener(GameEvent.SAVEDISK_ENTER, SaveDiskEnter);
    }

    private void SaveDiskEnter(object[] parameterContainer)
    {
        cameraPositionCount++;
        EventManager.RemoveEventListener(GameEvent.SAVEDISK_ENTER, SaveDiskEnter);
    }

    public void Enter()
    {
        _cam.transform.position = cameraPositions[0].position;
        _cam.transform.rotation = cameraPositions[0].rotation;
        _cam.enabled = true;
    }

    public void Execute()
    {
        transform.position = Vector3.Lerp(transform.position, cameraPositions[cameraPositionCount].position, Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, cameraPositions[cameraPositionCount].rotation, Time.deltaTime);
        if (Vector3.Distance(transform.position, cameraPositions[cameraPositionCount].position) < 0.5f)
        {
            EventManager.DispatchEvent(GameEvent.SAVEDISK_END);
        }
    }

    public void Exit()
    {
        _cam.enabled = false;
    }

    void OnDestroy()
    {
        EventManager.RemoveEventListener(GameEvent.SAVEDISK_ENTER, SaveDiskEnter);
    }
}
