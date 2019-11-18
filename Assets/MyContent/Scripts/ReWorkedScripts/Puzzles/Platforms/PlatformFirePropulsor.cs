using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformFirePropulsor : MonoBehaviour, IFlamableObjects {

    public direction dir;

    bool _isOnFire;
    public bool isOnFire { get { return _isOnFire; } set { _isOnFire = value; } }
    public bool absorvedFire;
    GameObject fireParticles;
    ParticleSystem[] _ps;
    public AnimationCurve curve;
    Transform absorverTransform;

    float _cooldown = 0.5f;
    float _cdTick;

    public void SetOnFire()
    {
        _isOnFire = true;
    }

    // Use this for initialization
    void Start ()
    {
        UpdatesManager.instance.AddUpdate(UpdateType.UPDATE, Execute);
        fireParticles = GameObject.Find("ParticleParent");
        fireParticles = fireParticles.transform.Find("Fire").gameObject;
        absorverTransform = transform.Find("AbsorverTransform");
        _ps = fireParticles.GetComponentsInChildren<ParticleSystem>();
        
    }
	
	// Update is called once per frame
	void Execute ()
    {
        if (_isOnFire || _cdTick < _cooldown)
        {
            for (int i = 0; i < _ps.Length; i++)
            {
                var particles = new ParticleSystem.Particle[_ps[i].particleCount];
                var count = _ps[i].GetParticles(particles);
                for (int j = 0; j < count; j++)
                {
                    particles[j].position = Vector3.Lerp(particles[j].position, absorverTransform.position, 1 - particles[j].remainingLifetime / particles[j].startLifetime);
                }

                _ps[i].SetParticles(particles, count);
                var solt = _ps[i].sizeOverLifetime;
                solt.size = new ParticleSystem.MinMaxCurve(1.5f, curve);
            }
            _cdTick += Time.deltaTime;
        }
        if (_isOnFire)
        {
            absorvedFire = true;
            _cdTick = 0;
        }

    }

    private void OnDestroy()
    {
        UpdatesManager.instance.RemoveUpdate(UpdateType.UPDATE, Execute);
    }

    public enum direction
    {
        X,
        X_NEGATIVE,
        Z,
        Z_NEGATIVE
    }
}
