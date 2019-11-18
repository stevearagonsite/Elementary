using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Skills;
using Player;

public class FireWall : MonoBehaviour, IVacuumObject
{
    public bool isAbsorved { get { return _isAbsorved; } set { _isAbsorved = value; } }
    public bool isAbsorvable { get { return _isAbsorvable; } }
    public bool isBeeingAbsorved { get { return _isBeeingAbsorved; } set { _isBeeingAbsorved = value; } }
    public Rigidbody rb { get { return _rb; } set { _rb = value; } }

    bool _isAbsorved;
    bool _isAbsorvable;
    bool _isBeeingAbsorved;
    Rigidbody _rb;

    ParticleSystem[] _ps;
    float t = 0.5f;
    float att = 0.1f;
    float blowAtt = 0.05f;

    public Transform vacuum;
    public AnimationCurve curve;

    public float damage;

    float initialFireAmount;
    public float fireAmount;
    public float fireRefillSpeed;
    BoxCollider _box;

    [Header("Cycle Variables")]
    public bool isCyclic;
    public float startDelay;
    public float period;
    [Range(0,1)]
    public float dutyCycle;

    bool isActive;
    float _activeTime;
    float _inactiveTime;
    float _offTick;
    float _onTick;
    float _startDelayTick;


    private void Start()
    {
        _ps = GetComponentsInChildren<ParticleSystem>();
        _rb = GetComponent<Rigidbody>();
        _box = GetComponent<BoxCollider>();
        initialFireAmount = fireAmount;
        _activeTime = period * dutyCycle;
        _inactiveTime = period * (1-dutyCycle);
        isActive = true;
        if (isCyclic)
        {
            UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
        }
    }

    void Execute()
    {
        if(_startDelayTick > startDelay)
        {
            if (isActive)
            {
                ActivateParticles();
                _offTick += Time.deltaTime;
                if (_offTick > _activeTime)
                {
                    isActive = false;
                    _offTick = 0;
                    _onTick = 0;
                    fireAmount = 0;
                    DeactivateParticles();
                }
            }
            else
            {
                _onTick += Time.deltaTime;
                if (_onTick > _inactiveTime)
                {
                    isActive = true;
                    _onTick = 0;
                    _offTick = 0;
                    fireAmount = initialFireAmount;
                    ActivateParticles();
                }
            }
        }
        else
        {
            _startDelayTick += Time.deltaTime;
        }
        
    }

    private void LateUpdate()
    {
        _rb.isKinematic = true;
    }

    public void BlowUp(Transform origin, float atractForce, Vector3 direction)
    {
        for (int i = 0; i < _ps.Length; i++)
        {
            var particles = new ParticleSystem.Particle[_ps[i].particleCount];
            var count = _ps[i].GetParticles(particles);

            for (int j = 0; j < count; j++)
            {
                var dir = -(vacuum.position - particles[j].position).normalized;
                particles[j].position += direction * atractForce * blowAtt * Time.deltaTime;
            }

            _ps[i].SetParticles(particles, count);
        }
    }

    public void SuckIn(Transform origin, float atractForce)
    {

        for (int i = 0; i < _ps.Length; i++)
        {
            var particles = new ParticleSystem.Particle[_ps[i].particleCount];
            var count = _ps[i].GetParticles(particles);
            for (int j = 0; j < count; j++)
            {
                particles[j].position = Vector3.Lerp(particles[j].position, vacuum.position, 1 - particles[j].remainingLifetime/ particles[j].startLifetime);
            }

            _ps[i].SetParticles(particles, count);
            var solt = _ps[i].sizeOverLifetime;
            solt.size = new ParticleSystem.MinMaxCurve(1.5f, curve);
        }

        origin.GetComponentInParent<SkillManager>().AddAmountToSkill(fireRefillSpeed * Time.deltaTime, Skills.Skills.FIRE);
        fireAmount -= fireRefillSpeed * Time.deltaTime;

        if(fireAmount > 0)
        {
            origin.GetComponentInParent<SkillManager>().AddAmountToSkill(fireRefillSpeed * Time.deltaTime, Skills.Skills.FIRE);
            fireAmount -= fireRefillSpeed * Time.deltaTime;
        }else
        {
            DeactivateParticles();
            gameObject.layer = 10;
            isActive = false;
        }

    }

    void DeactivateParticles()
    {
        for (int i = 0; i < _ps.Length; i++)
        {
            _ps[i].Stop();
        }
    }

    void ActivateParticles()
    {
        for (int i = 0; i < _ps.Length; i++)
        {
            _ps[i].Play();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (isActive)
        {
            var h = other.GetComponent<IHeat>();
            if (h != null)
            {
                h.Hit(damage);
            }
            else
            {
                var fO = other.GetComponent<IFlamableObjects>();
                if(fO != null)
                {
                    fO.SetOnFire();
                }
            }
        }
    }

    #region Unused IvacuumObjectMethods
    public void Shoot(float shootForce, Vector3 direction){}
    public void ReachedVacuum(){}
    public void ViewFX(bool active){}
    public void Exit(){}
    #endregion

    private void OnDestroy()
    {
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
    }
}
