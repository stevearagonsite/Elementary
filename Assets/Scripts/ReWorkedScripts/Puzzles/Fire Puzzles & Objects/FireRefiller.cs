using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Skills;

public class FireRefiller : MonoBehaviour, IVacuumObject
{
    bool _isAbsorved;
    bool _isAbsorvable;
    bool _isBeeingAbsorved;
    Rigidbody _rb;

    public ParticleSystem fireParticle;
    public float fireRefillSpeed;



    //Getter and Setters
    public bool isAbsorved
    {
        get{ return _isAbsorved; }
        set { _isAbsorved = value; }
    }

    public bool isAbsorvable
    {
        get { return _isAbsorvable; }
    }

    public bool isBeeingAbsorved
    {
        get { return _isBeeingAbsorved; }
        set { _isBeeingAbsorved = value; }
    }

    public Rigidbody rb
    {
        get { return _rb; }
        set { _rb = value; }
    }

    public void BlowUp(Transform origin, float atractForce, Vector3 direction)
    {

    }

    public void SuckIn(Transform origin, float atractForce)
    {
        origin.GetComponentInParent<SkillManager>().AddAmountToSkill(fireRefillSpeed * Time.deltaTime, Skills.Skills.FIRE);
    }

    public void ReachedVacuum(){}
    public void Shoot(float shootForce, Vector3 direction){}
    public void ViewFX(bool active){}
    public void Exit(){}

    void Start()
    {
        _isAbsorvable = false;
        _rb = GetComponent<Rigidbody>();
    }
}
