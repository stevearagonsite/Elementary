using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectToWeight : MonoBehaviour {

    Rigidbody _rb;
    public float mass;
    public Weight control;

    float _timmer = 2;
    [SerializeField]
    float _tick;

	void Start ()
    {
        _rb = GetComponent<Rigidbody>();
        mass = _rb.mass;
        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
    }

    private void Execute()
    {
        if(control != null)
        {
            _tick += Time.deltaTime;
            if(_tick>_timmer)
            {
                control.AddToWeight(this);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        var o = collision.collider.GetComponent<ObjectToWeight>();
        if (o != null)
        {
            control = o.control;
        }
        var w = collision.collider.GetComponent<Weight>();
        if (w != null)
        {
            control = w;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        var o = collision.collider.GetComponent<ObjectToWeight>();
        if(o != null)
        {
            control = o.control;
        }
        var w = collision.collider.GetComponent<Weight>();
        
        if(w != null)
        {
            control = w;
        }
        
    }

    private void OnCollisionExit(Collision collision)
    {
        var o = collision.collider.GetComponent<ObjectToWeight>();
        var w = collision.collider.GetComponent<Weight>();
        if (control != null &&(o != null || w!= null))
        {
            RemoveWeightFromControl();
            control = null;
        }
    }

    public void RemoveWeightFromControl()
    {
        if (control != null)
        {
            control.RemoveFromWeight(this);
            _tick = 0;
        }
    }

    private void OnDestroy()
    {
        RemoveWeightFromControl();
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
    }
}
