using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Volume))]
public class DeathFallTrigger : MonoBehaviour {

    public float timmer = 3;
    float _tick;
    Volume _deathPostProcess;
    bool _isActive;

    private void Start()
    {
        _deathPostProcess = GetComponent<Volume>();
        _deathPostProcess.weight = 0;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 9 && !_isActive)
        {
            _tick = 0;
            UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
            _isActive = true;
        }
         
    }

    void Execute()
    {
        _tick += Time.deltaTime;
         _deathPostProcess.weight = _tick/ timmer;
        if (_tick > timmer)
        {
            UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
            TransitionToRespawn();
        }
    }

    private void TransitionToRespawn()
    {
        EventManager.DispatchEvent(GameEvent.START_LEVEL_TRANSITION);
        EventManager.AddEventListener(GameEvent.TRANSITION_FADEOUT_WIN_FINISH, OnFadeOutEnd);

    }

    private void OnFadeOutEnd(object[] parameterContainer)
    {
        EventManager.RemoveEventListener(GameEvent.TRANSITION_FADEOUT_WIN_FINISH, OnFadeOutEnd);
        _tick = 0;
        _deathPostProcess.weight = 0;
        CheckPointManager.instance.RespawnPlayerInLastActiveCheckpoint();
        EventManager.DispatchEvent(GameEvent.TRANSITION_FADEIN_DEMO);
        _isActive = false;
    }

    private void OnDestroy()
    {
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
    }

    private void OnDrawGizmos()
    {
        var collider = GetComponent<BoxCollider>();
        Gizmos.color = new Color(1, 0, 0, 0.5f); ;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(collider.center, collider.size);
    }
}
