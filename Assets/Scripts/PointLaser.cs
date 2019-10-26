using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class PointLaser : MonoBehaviour {

    public float damage = 30;
    public GameObject laserInit;
    public GameObject laserEnd;
    LineRenderer _lineRenderer;
    BoxCollider _boxCollider;

    void Start () {

        _lineRenderer = GetComponent<LineRenderer>();
        _boxCollider = GetComponent<BoxCollider>();

        _lineRenderer.SetPosition(0, laserInit.transform.position);
        _lineRenderer.SetPosition(1, laserEnd.transform.position);
    }

    public bool GetState()
    {
        return _lineRenderer.enabled;
    }

    public void IsActive(bool value)
    {
        _lineRenderer.enabled = value;
        _boxCollider.enabled = value;
    }

    //private void OnTriggerEnter(Collider c)
    //{
    //    if (c.gameObject.layer == 9)
    //    {
    //        var pos = -c.gameObject.transform.forward;
    //        EventManager.DispatchEvent(GameEvent.PLAYER_TAKE_DAMAGE, damage);
    //        c.GetComponent<Rigidbody>().AddForce(pos * 300, ForceMode.Impulse);
    //    }
    //}
}
