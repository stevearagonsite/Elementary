using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;


public class FireVFX : IHandEffect
{
    ParticleSystem[] _particles;

    bool isPlaying;

    public FireVFX(ParticleSystem[] particles)
    {
        _particles = particles;
        foreach (var particle in _particles)
        {
            particle.Stop();
        }
    }

    public bool IsPlaying()
    {
        return isPlaying;
    }

    public void StartEffect()
    {
        StartEjectEffect();
    }

    public void StartEjectEffect()
    {
        
        if (!isPlaying)
        {
            foreach (var particle in _particles)
            {
                particle.Play();
                particle.gameObject.SetActive(true);
            }
            isPlaying = true;
        }
        
    }

    public void StopEffect()
    {
        foreach (var particle in _particles)
        {
            particle.Stop();
        }
        isPlaying = false;
        
    }

    public void TerminateEffect()
    {
        foreach (var particle in _particles)
        {
            particle.Stop();
            particle.gameObject.SetActive(false);
        }
        isPlaying = false;
        
    }

}
