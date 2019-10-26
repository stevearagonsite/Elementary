using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargeSizeObject : MonoBehaviour, IVacuumObject
{
    public float movement;

    [Header("Constrains")]
    public bool x;
    public bool z;


    bool _isAbsorved;
    bool _isAbsorvable;
    bool _isBeeingAbsorved;
    Rigidbody _rb;

    public bool isAbsorved { get { return _isAbsorved; } set { _isAbsorved = value; } }
    public bool isAbsorvable { get { return _isAbsorvable; } }
    public bool isBeeingAbsorved { get { return _isBeeingAbsorved; } set { _isBeeingAbsorved = value; } }
    public Rigidbody rb { get { return _rb; } set { _rb = value; } }

    void Start ()
    {
        _rb = GetComponent<Rigidbody>();
        _isAbsorvable = false;
	}

    public void BlowUp(Transform origin, float atractForce, Vector3 direction)
    {
        //Distance from vacuum to object
        RaycastHit ray;
        Physics.Raycast(origin.position, origin.forward, out ray, 2f);
        var distance = ray.distance;

        //Movement
        var dirx = x ? 0 : ray.normal.x;
        var dirz = z ? 0 : ray.normal.z;
        var dir = new Vector3(dirx, 0, dirz);
        movement = atractForce / _rb.mass;
        transform.position -= dir * movement * Time.deltaTime;

    }


    public void SuckIn(Transform origin, float atractForce)
    {
        //Distance from vacuum to object
        RaycastHit ray;
        Physics.Raycast(origin.position, origin.forward, out ray, 2f);
        var distance = ray.distance;

        //Movement
        if(distance > 0.2f)
        {
            var dirx = x ? 0 : ray.normal.x;
            var dirz = z ? 0 : ray.normal.z;
            var dir = new Vector3(dirx, 0, dirz);
            movement = atractForce / _rb.mass;
            transform.position += dir * movement * Time.deltaTime;
        }
        else
        {
            movement = 0;
        }
    }

    public void Exit()
    {
        isBeeingAbsorved = false;
    }

    public void ReachedVacuum(){}
    public void Shoot(float shootForce, Vector3 direction){}
    public void ViewFX(bool active){}

}
