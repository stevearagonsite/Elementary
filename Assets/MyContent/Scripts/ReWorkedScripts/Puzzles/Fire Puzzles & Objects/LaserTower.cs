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
    public float laserEndOffset;
    bool hasTargeted;
    Quaternion initialRotation;

    public Transform charger;
    public ParticleSystem[] particles;
    float chargerRotationSpeed;
    float maxChargerRotationSpeed = 360;

	void Start ()
    {
        _targets = new List<IHeat>();
        line = GetComponentInChildren<LineRenderer>();
        line.enabled = false;
        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].Stop();
        }
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
                line.enabled = false;
                for (int i = 0; i < particles.Length; i++)
                {
                    particles[i].Stop();
                }
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
        }
        charger.Rotate(new Vector3(0, 0, chargerRotationSpeed) * Time.deltaTime);
	}

    private void DrawLaser()
    {
        _delayTick += Time.deltaTime;
        bool somethingIsInTheWay = false;
        if(_delayTick > delay)
        {
            RaycastHit hit;
            if(Physics.Raycast(turretLaser.transform.position, (_laserTarget.Transform.position - turretLaser.position).normalized,out hit, 10))
            {
                var iHeatObj = hit.collider.GetComponent<IHeat>();
                if(iHeatObj == null)
                {
                    iHeatObj = hit.collider.GetComponentInChildren<IHeat>();
                }
                if(iHeatObj != null)
                {
                    iHeatObj.Hit(damage);
                    somethingIsInTheWay = true;
                    Debug.Log(hit.collider.name);
                }
            }
            if(!somethingIsInTheWay)
                _laserTarget.Hit(damage);

            if (!hasTargeted)
            {
                line.enabled = true;
                hasTargeted = true;
            }
            var dist = Vector3.Distance(hit.collider.transform.position, turretLaser.position) + laserEndOffset;
            line.SetPosition(line.positionCount - 1, new Vector3(0, 0, dist));
        }
        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].Play();
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
