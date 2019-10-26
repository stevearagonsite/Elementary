
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FencePuzzleAbsorver : MediumSizeObject, IVacuumObject
{
    BoxCollider _bc;

    new void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _bc = GetComponent<BoxCollider>();

        _bc.material.dynamicFriction = 0.6f;
        base.Start();
        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
    }

    void Execute()
    {
        if (_isBeeingAbsorved)
        {
            _bc.material.dynamicFriction = 0.0f;
        }
        else
        {
            _bc.material.dynamicFriction = 0.6f;
        }
    }

    public new void BlowUp(Transform origin, float atractForce, Vector3 direction)
    {
        var distance = (transform.position - origin.position).magnitude;
        var forceMagnitude = rb.mass * atractForce * 10 / Mathf.Pow(distance, 2);
        forceMagnitude = Mathf.Clamp(forceMagnitude, 0, 2000);
        var force = direction.normalized * forceMagnitude;
        rb.AddForce(force);
    }

    public new void SuckIn(Transform origin, float atractForce)
    {
        var direction = origin.position - transform.position;
        var distance = direction.magnitude;
        var forceMagnitude = (rb.mass) * atractForce / distance;
        var force = direction.normalized * forceMagnitude;
        rb.AddForce(force);
    }

    private new void OnDestroy()
    {
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
        base.OnDestroy();
    }
    public new void Exit(){}
    public new void ViewFX(bool activate){}

}
