using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Skills;
using Player;
using System;

public class PlayerTemperature : MonoBehaviour,IHeat
{
    [SerializeField]
    float _temperature;

    public float temperature { get { return _temperature; } }
    public Transform Transform { get { return transform; } }

    public SkinnedMeshRenderer[] renderers;

    public float timeToFreeze;
    private float _tick;

    public float lifeLeft { get { return Mathf.Clamp01( _life / life); } }

    public float life;
    float _life;

    bool _setToDieByLaser;



    private TPPController _playerController;

    void Start()
    {
        _playerController = GetComponentInParent<TPPController>();
        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
        _life = life;
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.SetFloat("_FrozenAmount", 0);
        }
    }

    public void Restart()
    {
        _setToDieByLaser = false;
        _life = life;
        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.SetFloat("_FrozenAmount", 0);
        }
    }

    void Execute()
    {
        if (_setToDieByLaser)
        {
            Die();
            UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
        }
    }

    private void Die()
    {
        _playerController.isActive = false;
        _tick = 0;
        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Freeze);
        
    }

    private void Freeze()
    {
        if(_tick < timeToFreeze)
        {
            _tick += Time.deltaTime;
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material.SetFloat("_FrozenAmount", _tick / timeToFreeze);
            }
        }
        else
        {
            UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Freeze);
            Debug.Log("Murio congela3");
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
        CheckPointManager.instance.RespawnPlayerInLastActiveCheckpoint();
        EventManager.DispatchEvent(GameEvent.TRANSITION_FADEIN_DEMO);
        Restart();
    }

    public void Hit(float damage)
    {
        if (!_setToDieByLaser)
        {
            _life -= damage * Time.deltaTime;
            if (_life < 0)
            {
                _setToDieByLaser = true;
            }
        }
    }

    private void OnDestroy()
    {
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
    }
}
