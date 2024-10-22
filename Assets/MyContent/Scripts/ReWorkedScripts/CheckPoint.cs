﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CheckPoint : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        CheckPointManager.instance.RegisterActiveCheckPoint(this);
        Debug.Log("Registe on enter" + gameObject.name);
    }

    private void OnTriggerStay(Collider other)
    {
        CheckPointManager.instance.RegisterActiveCheckPoint(this);
        Debug.Log("Registe on stay" + gameObject.name);
    }

    private void OnDrawGizmos()
    {
        var collider = GetComponent<BoxCollider>();
        Gizmos.color = new Color(0, 200, 0, 0.7f); ;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(collider.center, collider.size);
    }
}
