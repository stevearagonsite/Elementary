using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTower : MonoBehaviour
{

    List<IHeat> _targets;
    public float targetTemperature;
    public float damage;
    IHeat _laserTarget;
    IHeat _lastTarget;

    public float delay = 2;
    [SerializeField]
    float _delayTick;

    LineRenderer line;

    public Transform turretLaser;
    ElectricParticleEmitter emitter;
    bool hasTargeted;
    Quaternion initialRotation;

    public Transform charger;
    float chargerRotationSpeed;
    float maxChargerRotationSpeed = 360;

	void Start ()
    {
        _targets = new List<IHeat>();
        line = GetComponent<LineRenderer>();
        line.enabled = false;
        emitter = GetComponent<ElectricParticleEmitter>();
        initialRotation = transform.rotation;
        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
	}
	
	
	void Execute ()
    {
       
		if(_targets != null && _targets.Count > 0)
        {
            foreach (var t in _targets)
            {
                if(t.temperature > targetTemperature)
                {
                    if(_laserTarget == null || _laserTarget.temperature < t.temperature)
                    {
                        _laserTarget = t;
                        _delayTick = 0;
                    }
                    
                }
            }
        }
        else
        {
            _laserTarget = null;
        }
        if (_laserTarget != null)
        {
            var dir = -(_laserTarget.Transform.position - turretLaser.transform.position).normalized;
            var targetRot = Quaternion.LookRotation(dir);
            turretLaser.rotation = Quaternion.Slerp(turretLaser.rotation, targetRot, 0.5f);
            chargerRotationSpeed = Mathf.Lerp(chargerRotationSpeed, maxChargerRotationSpeed, 0.5f);
            if(_laserTarget != _lastTarget)
            {
                _delayTick = 0;
                _lastTarget = _laserTarget;
                hasTargeted = false;
                emitter.StopEffect();
                line.enabled = false;
            }
            else
            {
                DrawLaser();
            }
        }
        else
        {
            _delayTick = 0;
            line.enabled = false;
            turretLaser.rotation = Quaternion.Slerp(turretLaser.rotation, initialRotation, 0.2f);
            chargerRotationSpeed = Mathf.Lerp(chargerRotationSpeed, 0, 0.2f);
            hasTargeted = false;
            emitter.StopEffect();
        }
        charger.Rotate(new Vector3(0, 0, chargerRotationSpeed) * Time.deltaTime);
	}

    private void DrawLaser()
    {
        _delayTick += Time.deltaTime;
        if(_delayTick > delay)
        {
            _laserTarget.Hit(damage);
            emitter.Initialize(_laserTarget.Transform);
            if (!hasTargeted)
            {
                emitter.StartEffect();
                line.enabled = true;
                hasTargeted = true;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        var h = other.GetComponent<IHeat>();
        if (h != null)
        {
            
            if (!_targets.Contains(h))
            {
                _targets.Add(h);
            }
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        var h = other.GetComponent<IHeat>();
        if(h != null)
        {
            if (_targets.Contains(h))
            {
                if(_laserTarget == h)
                {
                    _laserTarget = null;
                    line.enabled = false;
                }
                _targets.Remove(h);
                
            }
        }
    }
}
