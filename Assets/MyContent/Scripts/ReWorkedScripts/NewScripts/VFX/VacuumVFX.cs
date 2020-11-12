using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;


public class VacuumVFX : IHandEffect {

    VisualEffect _absorb;
    VisualEffect _blow;

    bool _isPlaying;
    bool _isPlayingBlow;

    public VacuumVFX(VisualEffect absorb, VisualEffect blow)
    {
        _absorb = absorb;
        _blow = blow;

        _absorb.Stop();
        _blow.Stop();
    }

    public bool IsPlaying()
    {
        return _isPlaying;
    }

    public void StartEffect()
    {
        if (_absorb)
        {
            if (!_isPlaying)
            {
                _absorb.gameObject.SetActive(true);
                _absorb.Play();
                _isPlaying = true;
            }
        }
    }

    public void StartEjectEffect() 
    {
        if (_blow)
        {
            if (!_isPlayingBlow)
            {
                _blow.gameObject.SetActive(true);
                _blow.Play();
                _isPlayingBlow = true;
            }
        }
    }

    public void StopEffect()
    {
        if (_absorb)
        {
            _absorb.Stop();
        }
        if (_blow) 
        {
            _blow.Stop();
        }
        _isPlaying = false;
        _isPlayingBlow = false;
    }

    public void TerminateEffect()
    {
        if (_absorb)
        {
            _absorb.gameObject.SetActive(false);  
        }
        if (_blow) 
        {
            _blow.gameObject.SetActive(false);
        }
        _isPlaying = false;
        _isPlayingBlow = false;
    }

}
