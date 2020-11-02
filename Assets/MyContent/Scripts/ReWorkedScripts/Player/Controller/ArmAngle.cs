﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Skills;
using System;
using Player;

public class ArmAngle : MonoBehaviour {

    IKControl ikControl;
    bool _isActive;

    public Transform armPivot;
    Transform cameraT;

    void Start ()
    {
        ikControl = GetComponent<IKControl>();
        InputManager.instance.AddAction(InputType.Absorb, Absorb);
        InputManager.instance.AddAction(InputType.Reject, Absorb);
        InputManager.instance.AddAction(InputType.Stop, Stop);
        cameraT = GetComponentInParent<TPPController>().cameraT.transform;
        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
    }

    private void Absorb() 
    {
        _isActive = true;
    }

    private void Stop()
    {
        _isActive = false;
    }

    private void Execute() 
    {
        if (_isActive) 
        {
            var x = cameraT.localEulerAngles.x;
            armPivot.localEulerAngles = new Vector3(x, 0, 0);
            ikControl.ikActive = true;
            _isActive = false;
        }
        else
        {
            ikControl.ikActive = false;
        }
    }

    void OnDestroy()
    {
        InputManager.instance.RemoveAction(InputType.Absorb, Absorb);
        InputManager.instance.RemoveAction(InputType.Reject, Absorb);
        InputManager.instance.RemoveAction(InputType.Stop, Stop);
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);

    }
}
