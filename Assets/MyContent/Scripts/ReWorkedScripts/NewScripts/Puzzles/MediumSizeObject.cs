using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Material))]
[RequireComponent(typeof(Rigidbody))]
public class MediumSizeObject : MonoBehaviour, IVacuumObject
{

    [HideInInspector]
    public bool wasShooted;

    [HideInInspector]
    public Material material;//Edit for shoot vfx.
    private BoxCollider _bC;

    float _alphaCut;

    Vector3 _initialPosition;
    public float respawnDistance;
    public float respawnTime;

    float _respawnTick;

    float _disolveTimmer = 1;
    float _disolveTick;
    bool _disolve;

    protected bool _isAbsorved;
    protected bool _isAbsorvable;
    protected bool _isBeeingAbsorved;
    protected Rigidbody _rb;

    public bool isAbsorved { get { return _isAbsorved; } set { _isAbsorved = value; } }
    public bool isAbsorvable { get { return _isAbsorvable; } }
    public bool isBeeingAbsorved { get { return _isBeeingAbsorved; } set { _isBeeingAbsorved = value; } }
    public Rigidbody rb { get { return _rb; } set { _rb = value; } }

    public bool respawnable;

    protected void Start()
    {
        _initialPosition = transform.position;
        _isAbsorvable = false;
        _rb = GetComponent<Rigidbody>();
        material = GetComponent<Renderer>().material;
        _bC = GetComponent<BoxCollider>();
        SpawnVFXActivate(true);

    }

    void Execute()
    {
        var d = Mathf.Abs((transform.position - _initialPosition).magnitude);
        if (d > respawnDistance)
        {
            wasShooted = false;
            _disolve = false;
            SpawnVFXActivate(false);
        }
    }

    public void SpawnVFXActivate(bool dir)
    {
        if (dir)
        {
            _alphaCut = 1;
            UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, SpawnVFX);
            if (respawnable)
                UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
        }
        else
        {
            _alphaCut = 0;
            UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, DespawnVFX);
            if (respawnable)
                UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
        }
    }

    void SpawnVFX()
    {
        if(_respawnTick >= respawnTime)
        {
            material.SetFloat("_DisolveAmount", _alphaCut);
            _alphaCut -= Time.deltaTime;
            _rb.useGravity = true;
            _bC.isTrigger = false;
            if (_alphaCut <= 0)
            {
                UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, SpawnVFX);
                wasShooted = false;
                _respawnTick = 0;
            }
        }
        else
        {
            _respawnTick += Time.deltaTime;
        }
        
    }

    void DespawnVFX()
    {
        material.SetFloat("_DisolveAmount", _alphaCut);
        _alphaCut += Time.deltaTime;
        if (_alphaCut >= 1)
        {
            UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, DespawnVFX);
            RepositionOnSpawn();
        }
    }

    public void RepositionOnSpawn()
    {
        transform.position = _initialPosition;
        transform.rotation = Quaternion.identity;
        rb.velocity = Vector3.zero;
        _rb.useGravity = false;
        _bC.isTrigger = true;
        SpawnVFXActivate(true);
        wasShooted = true;

        //for box temperature
        var bt = GetComponent<BoxTemperature>();
        if (bt)
        {
            bt.ResetBox();
        }
    }

    public void SuckIn(Transform origin, float atractForce)
    {
        if (!wasShooted)
        {
            var direction = origin.position - transform.position;
            var distance = direction.magnitude;

            if (distance <= 0.7f)
            {
                _bC.isTrigger = true;
                rb.isKinematic = true;
                transform.position = origin.position;
                isAbsorved = true;
                transform.SetParent(origin);

            }
            else if (distance < 1f)
            {
                rb.isKinematic = true;

                var dir = (origin.position - transform.position).normalized;
                transform.position += dir * atractForce / 10 * Time.deltaTime;
            }
            else
            {
                rb.isKinematic = false;
                var forceMagnitude = (10) * atractForce / Mathf.Pow(distance, 2);
                var force = direction.normalized * forceMagnitude;
                rb.AddForce(force);

            }
        }
    }

    public void BlowUp(Transform origin, float atractForce, Vector3 direction)
    {
        if (!wasShooted)
        {
            rb.isKinematic = false;
            isAbsorved = false;
            transform.SetParent(null);
            var distanceRaw = transform.position - origin.position;
            var distance = distanceRaw.magnitude;
            var forceMagnitude = 10 * atractForce * 10 / Mathf.Pow(distance, 2);
            forceMagnitude = Mathf.Clamp(forceMagnitude, 0, 2000);
            var force = direction.normalized * forceMagnitude;
            rb.AddForce(force);
        }
    }

    public void ReachedVacuum()
    {

    }

    public void Shoot(float shootForce, Vector3 direction)
    {
        _bC.isTrigger = false;
        wasShooted = true;
        isAbsorved = false;
        rb.isKinematic = false;
        transform.SetParent(null);
        rb.velocity = direction * shootForce / rb.mass;
        _disolveTick = 0;
    }

    public void ViewFX(bool activate)
    {
        if (activate)
        {
            //View VFX

            material.SetFloat("_Alpha", 0.3f);
        }
        else
        {
            //Reset view VFX
            material.SetFloat("_Alpha", 1f);
        }
    }

    public void Exit()
    {
        _bC.isTrigger = false;
        ViewFX(false);
        transform.SetParent(null);
        rb.isKinematic = false;
        isAbsorved = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //TODO: Find a better way to exclude "Player" collision
        if(collision.gameObject.name != "Player")
        {
            wasShooted = false;
        }
    }

    private void OnCollisionStay(Collision collision)
    {

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, respawnDistance);
    }

    protected void OnDestroy()
    {
        if (respawnable)
            UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(200, 200, 200, 0.6f); ;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(Vector3.zero, Vector3.one);
    }
}
