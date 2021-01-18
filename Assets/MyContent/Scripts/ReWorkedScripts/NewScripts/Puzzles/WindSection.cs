using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindSection : MonoBehaviour
{
    public float windPower;
    public float activeTime;
    public float inactiveTime;

    public ParticleSystem windParticle;


    private bool _isPlayingWind;
    private float _tick;
    
    private CharacterController _cc;

    private void Start()
    {
        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
        windParticle.Stop();
    }

    private void Execute()
    {
        if(_tick > inactiveTime + activeTime)
        {
            _tick = 0;
            _isPlayingWind = false;
            windParticle.Stop();
        }
        if (_tick > inactiveTime)
        {
            if (_cc != null)
            {
                _cc.Move(transform.forward * windPower * Time.deltaTime);
                Debug.Log("Muevo");
            }
            if (!_isPlayingWind)
            {
                windParticle.Play();
                _isPlayingWind = true;
            }
        }
        _tick += Time.deltaTime;

    }

    private void OnTriggerEnter(Collider other)
    {
        _cc = other.GetComponent<CharacterController>();
        Debug.Log("Entro");
    }

    private void OnTriggerExit(Collider other)
    {
        _cc = null;
        Debug.Log("Salgo");
    }

    private void OnDrawGizmos()
    {
        var collider = GetComponent<BoxCollider>();
        Gizmos.color = new Color(0.5f, 1, 0.5f, 0.7f); ;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(collider.center, collider.size);
    }
}
