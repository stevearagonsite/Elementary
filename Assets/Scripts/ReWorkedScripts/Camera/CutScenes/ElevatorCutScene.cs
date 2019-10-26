using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorCutScene : MonoBehaviour, CutScene
{
    Camera cam;
    [Header("Camera positions")]
    public Transform[] cameraPositions;

    int cameraCount = 0;
    float _timmer = 2;
    float _tick;

    bool startedFadedOut;
    CutSceneCamera _cutSceneCamera;

    public Animator blackOutAnimator;

    [Header("Transforms references")]
    public Transform player;
    public Transform elevatorTransform;
    public Transform objective;

    void Start()
    {
        cam = GetComponent<Camera>();
        _cutSceneCamera = GetComponent<CutSceneCamera>();
        cam.enabled = false;
    }

    public void Enter()
    {
        cam.enabled = true;
        EventManager.AddEventListener(GameEvent.TRANSITION_FADEOUT_DEMO, FadeOutEnd);
        blackOutAnimator.SetTrigger("FadeOutAndIn");
    }

    public void Execute()
    {
        transform.position = cameraPositions[cameraCount].position;
        transform.rotation = cameraPositions[cameraCount].rotation;
        if (_tick < _timmer)
        {
            _tick += Time.deltaTime;
        }
        else if (!startedFadedOut)
        {
            blackOutAnimator.SetTrigger("FadeOutAndIn");
            startedFadedOut = true;
        }
    }

    public void Exit()
    {
        cam.enabled = false;
    }

    void FadeOutEnd(object[] parameterContainer)
    {
        player.position = elevatorTransform.position;
        player.rotation = elevatorTransform.rotation;
        cameraCount++;
        if (cameraCount > 1)
        {
            EventManager.RemoveEventListener(GameEvent.TRANSITION_FADEOUT_DEMO, FadeOutEnd);
            EventManager.AddEventListener(GameEvent.TRANSITION_FADEIN_DEMO, FadeInEnd);
            player.position = objective.position;
            player.rotation = objective.rotation;
        }
    }

    void FadeInEnd(object[] parameterContainer)
    {
        EventManager.RemoveEventListener(GameEvent.TRANSITION_FADEIN_DEMO, FadeInEnd);
        EventManager.DispatchEvent(GameEvent.CAMERA_NORMAL);
        CutScenesManager.instance.DeActivateCutSceneCamera(_cutSceneCamera.cutSceneTag);
    }

    void OnDestroy()
    {
        EventManager.RemoveEventListener(GameEvent.TRANSITION_FADEOUT_DEMO, FadeOutEnd);
        EventManager.RemoveEventListener(GameEvent.TRANSITION_FADEIN_DEMO, FadeInEnd);
    }
}
