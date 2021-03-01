using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attractor : ISkill {

    public List<IVacuumObject> _objectsToInteract;

    float _atractForce;
    float _shootSpeed;
    Transform _vacuumHoleTransform;

    SkinnedMeshRenderer _targetMesh;
    Mesh _atractorMesh;

    bool _isStuck;
    public bool isStuck { get { return _isStuck; } }

    PathCalculate _pc;

    public Attractor(float atractForce, float shootSpeed, Transform vacuumHole, List<IVacuumObject> objectsToInteract)
    {
        _atractForce = atractForce;
        _shootSpeed = shootSpeed;
        _vacuumHoleTransform = vacuumHole;
        _objectsToInteract = objectsToInteract;

    }

    public void Enter(){}

    public void Absorb()
    {

        if (_isStuck)
        {
            if (_objectsToInteract.Count > 0)
                _objectsToInteract[0].ViewFX(true);
            else
            {
                EventManager.DispatchEvent(GameEvent.VACUUM_FREE);
                _isStuck = false;
            }
        }
        Attract();
    }

    public void Eject()
    {
        if (_isStuck)
        {
            if (_objectsToInteract.Count > 0)
            {
                _objectsToInteract[0].ViewFX(false);
                _objectsToInteract[0].Shoot(_shootSpeed, _vacuumHoleTransform.forward);
            }
            _isStuck = false;
            EventManager.DispatchEvent(GameEvent.VACUUM_FREE);
        }
        else
        {
            Reject();
        }
    }
    

    public void Exit()
    {
        _isStuck = false;
        foreach (var obj in _objectsToInteract)
        {
            obj.Exit();
        }
        _objectsToInteract.Clear();
    }

    void Attract ()
    {
        for (int i = 0; i < _objectsToInteract.Count; i++)
        {
            if (!_isStuck)
            {
                _objectsToInteract[i].SuckIn(_vacuumHoleTransform, _atractForce);
                _objectsToInteract[i].isBeeingAbsorved = true;
            }
            if (_objectsToInteract[i].isAbsorved && _objectsToInteract[i].isAbsorvable)
            {
                _objectsToInteract[i].ReachedVacuum();
                _objectsToInteract.Remove(_objectsToInteract[i]);
            }
            else if (_objectsToInteract[i].isAbsorved && !_objectsToInteract[i].isAbsorvable)
            {
                var aux = _objectsToInteract[i];
                for (int j = 0; j < _objectsToInteract.Count; j++)
                {
                    if(_objectsToInteract[j] != aux)
                    {
                        _objectsToInteract[j].isBeeingAbsorved = false;
                        _objectsToInteract[j].Exit(); 
                    }
                }
                _objectsToInteract.RemoveAll(x => x != null);
                _objectsToInteract.Add(aux);
                _isStuck = true;
                EventManager.DispatchEvent(GameEvent.VACUUM_STUCK);
            }
        }
    }

    void Reject()
    {
        _isStuck = false;
        if (_objectsToInteract.Count > 0)
        {
            foreach (var obj in _objectsToInteract)
            {
                obj.BlowUp(_vacuumHoleTransform, _atractForce, _vacuumHoleTransform.forward);
                obj.isBeeingAbsorved = true;
            }

        }
    }
}
