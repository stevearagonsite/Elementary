using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;


public class FireVFX : IHandEffect
{

    VisualEffect _particle;


    bool isPlaying;

    public FireVFX(VisualEffect particle)
    {
        _particle = particle;
        particle.Stop();
    }

    public bool IsPlaying()
    {
        return isPlaying;
    }

    public void StartEffect()
    {
        
    }

    public void StartEjectEffect()
    {
        if (_particle)
        {
            if (!isPlaying)
            {
                _particle.gameObject.SetActive(true);
                _particle.Play();
                isPlaying = true;
            }
        }
    }

    public void StopEffect()
    {
        if (_particle)
        {
            _particle.Stop();
            isPlaying = false;
        }
    }

    public void TerminateEffect()
    {
        if (_particle)
        {
            _particle.gameObject.SetActive(false);
            isPlaying = false;
        }
    }

}
