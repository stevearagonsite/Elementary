using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Follower : MonoBehaviour
{
    public string gameObjectToFollowName;

    private bool _isActive;
    private bool _isAssigned;

    private GameObject _goToFollow;
    private VisualEffect _vfx;

    void Start()
    {
        _vfx = GetComponent<VisualEffect>();
        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
        EventManager.AddEventListener(GameEvent.DOUBLE_STORM_PLAY, ThickerStorm);
        EventManager.AddEventListener(GameEvent.DOUBLE_STORM_STOP, ThinerStorm);
        _vfx.SendEvent("DoubleStormPlay");
    }

    private void ThinerStorm(object[] p)
    {
        _vfx.SendEvent("DoubleStormStop");
    }

    private void ThickerStorm(object[] p)
    {
        _vfx.SendEvent("DoubleStormPlay");
    }

    void Execute()
    {
        if (!_isAssigned)
        {
            _goToFollow = GameObject.Find(gameObjectToFollowName);
            if(_goToFollow != null) 
            {
                _isAssigned = true;
            }
        }
        else
        {
            transform.position = _goToFollow.transform.position;
        }

        
    }

    private void OnDestroy()
    {
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
        EventManager.RemoveEventListener(GameEvent.DOUBLE_STORM_PLAY, ThickerStorm);
        EventManager.RemoveEventListener(GameEvent.DOUBLE_STORM_STOP, ThinerStorm);
    }
}
