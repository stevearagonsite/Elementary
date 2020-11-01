using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attractor : ISkill {

    public List<IVacuumObject> _objectsToInteract;

    float _atractForce;
    float _shootSpeed;
    Transform _vacuumHoleTransform;
    IHandEffect _aspireParticle;
    IHandEffect _blowParticle;

    SkinnedMeshRenderer _targetMesh;
    Mesh _atractorMesh;

    bool _isStuck;
    public bool isStuck { get { return _isStuck; } }

    PathCalculate _pc;

    public Attractor(float atractForce, float shootSpeed, Transform vacuumHole, IHandEffect aspireParticle,
                    IHandEffect blowParticle, List<IVacuumObject> objectsToInteract)
    {
        _atractForce = atractForce;
        _shootSpeed = shootSpeed;
        _vacuumHoleTransform = vacuumHole;
        _aspireParticle = aspireParticle;
        _blowParticle = blowParticle;
        _objectsToInteract = objectsToInteract;

        _aspireParticle.StopEffect();
        _blowParticle.StopEffect();

    }

    public void Enter(){}

    public void Absorb()
    {
        _blowParticle.StopEffect();
        if (_isStuck)
        {
            _aspireParticle.StopEffect();
            _isStuck = false;
            if (_objectsToInteract.Count > 0)
                _objectsToInteract[0].ViewFX(true);
        }
        else
        {
            if (!_aspireParticle.IsPlaying() && !_isStuck)
                _aspireParticle.StartEffect();
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
        }
        else
        {
            _aspireParticle.StopEffect();
            _isStuck = false;
            Reject();
            if (!_blowParticle.IsPlaying())
                _blowParticle.StartEffect();
        }
    }
    

    public void Exit()
    {
        _aspireParticle.StopEffect();
        _blowParticle.StopEffect();
        _isStuck = false;
        foreach (var obj in _objectsToInteract)
        {
            obj.Exit();
        }
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
                _objectsToInteract.RemoveAll(x => x != null);
                _objectsToInteract.Add(aux);
                _isStuck = true;
                _aspireParticle.StopEffect();
            }
        }
        Debug.Log("Attract: " + _objectsToInteract.Count);
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
