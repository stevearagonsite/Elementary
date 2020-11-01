using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Skills;
using System;
using Player;

public class ArmAngle : MonoBehaviour {

    public float MAX_Y_ANGLE = 40f;
    public float MIN_Y_ANGLE = -10f;

    float _currentY;

    IKControl ikControl;
    bool _isActive;

    public Transform armPivot;
    Transform cameraT;

    void Start ()
    {
        ikControl = GetComponent<IKControl>();
        InputManager.instance.AddAction(InputType.Absorb, Absorb);
        cameraT = GetComponentInParent<TPPController>().cameraT.transform;
        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
    }

    private void Absorb() 
    {
        _isActive = true;
    }

    private void Execute() 
    {
        if (_isActive) 
        {
            //var f = (armPivot.position - cameraT.position).normalized;
            //armPivot.forward = f;
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
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
    }
}
