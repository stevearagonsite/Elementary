using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TPCamera;
using System;

public class CameraManager : MonoBehaviour
{
    private static CameraManager _instance;
    public static CameraManager instance { get { return _instance; } }

    private List<Camera> _registeredCameras;
    private CameraFSM _mainCamera; 

    private void Awake()
    {
        if (_instance == null) _instance = this;
        _registeredCameras = new List<Camera>();
    }

    void Start()
    {
        EventManager.AddEventListener(GameEvent.CAMERA_STORY, OnCameraStory);
        EventManager.AddEventListener(GameEvent.CAMERA_NORMAL, OnCameraNormal);
    }

    private void OnCameraNormal(object[] parameterContainer)
    {
        StartCoroutine(GoToMainCamera());
    }
    IEnumerator GoToMainCamera()
    {
        yield return new WaitForSeconds(0.5f);
        foreach (var cam in _registeredCameras)
        {
            cam.enabled = false;
            cam.GetComponent<Animator>().Play("idle");
        }
        if (_mainCamera != null)
        {
            _mainCamera.enabled = true;
            _mainCamera.GetComponent<Camera>().enabled = true;
            _mainCamera.GoToStartPosition();
        }
    }

    private void OnCameraStory(object[] p)
    {
        StartCoroutine(GoToStory(p));
    }

    IEnumerator GoToStory(object[] p)
    {
        yield return new WaitForSeconds(0.5f);
        var camToActivate = p[0] as Camera;
        foreach (var cam in _registeredCameras)
        {
            cam.enabled = false;
            cam.GetComponent<Animator>().Play("idle");
        }
        if (_mainCamera != null)
        {
            _mainCamera.enabled = false;
            _mainCamera.GetComponent<Camera>().enabled = false;
        }
        camToActivate.enabled = true;
        camToActivate.GetComponent<Animator>().Play("story");
    }

    public void RegisterCamera(Camera cam)
    {
        if (!_registeredCameras.Contains(cam))
        {
            _registeredCameras.Add(cam);
        }
    }

    public void RegisterMainCamera(CameraFSM mainCam)
    {
        _mainCamera = mainCam;
    }

    public void RemoveCameraFromRegister(Camera cam)
    {
        if (_registeredCameras.Contains(cam))
        {
            _registeredCameras.Remove(cam);
        }
    }

    public void UnregisterMainCamera()
    {
        _mainCamera = null;
    }

    private void OnDestroy()
    {
        EventManager.RemoveEventListener(GameEvent.CAMERA_STORY, OnCameraStory);
        EventManager.RemoveEventListener(GameEvent.CAMERA_NORMAL, OnCameraNormal);
    }
}
