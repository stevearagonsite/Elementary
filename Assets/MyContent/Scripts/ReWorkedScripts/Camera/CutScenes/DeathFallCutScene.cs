using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathFallCutScene : MonoBehaviour, CutScene {

    public Camera mainCamera;
    public Transform target;
    Camera _cam;

    

    

    void Start()
    {
        _cam = GetComponent<Camera>();
        _cam.enabled = false;
    }


    public void Enter()
    {
        transform.position = mainCamera.transform.position;
        transform.rotation = mainCamera.transform.rotation;
        _cam.enabled = true;
    }

    public void Execute()
    {
        transform.LookAt(target);
        transform.position -= transform.forward * Time.deltaTime * 3f;
    }

    public void Exit()
    {
        _cam.enabled = false;
    }

    private void OnDestroy()
    {
    }

}
