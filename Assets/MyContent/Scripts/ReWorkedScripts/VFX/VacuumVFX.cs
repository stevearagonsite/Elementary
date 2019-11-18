using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VacuumVFX : IHandEffect {

    ParticleSystem particle;
    bool isPlaying;
    Transform _particleParent;
    Transform _vacuumHole;
    Vector3 _positionOffset;

    //0.003949951, 0.002224207, 1.2931

    public VacuumVFX(ParticleSystem particle, Transform particleParent, Transform vacuumHole)
    {
        this.particle = particle;
        _particleParent = particleParent;
        _vacuumHole = vacuumHole;
    }

    public VacuumVFX(ParticleSystem particle, Transform particleParent, Transform vacuumHole, Vector3 positionOffset)
    {
        this.particle = particle;
        _particleParent = particleParent;
        _vacuumHole = vacuumHole;
        _positionOffset = positionOffset;
    }

    public bool IsPlaying()
    {
        return isPlaying;
    }

    public void StartEffect()
    {
        if (!isPlaying)
        {
            particle.gameObject.SetActive(true);
            particle.Play();
            particle.gameObject.transform.SetParent(_vacuumHole);
            particle.gameObject.transform.localPosition = Vector3.zero;
            if(_positionOffset != null)
            {
                particle.gameObject.transform.localPosition += _positionOffset;
            }
            particle.gameObject.transform.localRotation = Quaternion.identity;
            isPlaying = true;
        }
    }

    public void StopEffect()
    {
        particle.Stop();
        particle.gameObject.transform.SetParent(_particleParent);
        isPlaying = false;
    }

    public void TerminateEffect()
    {
        particle.gameObject.SetActive(false);
        particle.gameObject.transform.SetParent(_particleParent);
        isPlaying = false;
    }

}
