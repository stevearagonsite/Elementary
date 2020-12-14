using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;


public class VacuumVFX : IHandEffect {

    GameObject _absorb;
    GameObject _blow;

    bool _isPlaying;
    bool _isPlayingBlow;

    List<TornadoVFXController> _absorbTornadoes;
    List<ParticleSystem> _absorbParticles;

    List<TornadoVFXController> _rejectTornadoes;
    List<ParticleSystem> _rejectParticles;

    public VacuumVFX(GameObject absorb, GameObject blow)
    {
        _absorb = absorb;
        _blow = blow;

        _absorbTornadoes = new List<TornadoVFXController>();
        _absorbParticles = new List<ParticleSystem>();

        for (int i = 0; i < _absorb.transform.childCount; i++)
        {
            var go = _absorb.transform.GetChild(i);
            var ps = go.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                _absorbParticles.Add(ps);
                ps.Stop();
            }
            else
            {
                var tvfx = go.GetComponent<TornadoVFXController>();
                _absorbTornadoes.Add(tvfx);
                tvfx.StopEffect();

            }
        }

        _rejectTornadoes = new List<TornadoVFXController>();
        _rejectParticles = new List<ParticleSystem>();

        for (int i = 0; i < _blow.transform.childCount; i++)
        {
            var go = _blow.transform.GetChild(i);
            var ps = go.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                _rejectParticles.Add(ps);
                ps.Stop();
            }
            else
            {
                var tvfx = go.GetComponent<TornadoVFXController>();
                _rejectTornadoes.Add(tvfx);
                tvfx.StopEffect();
            }
        }
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
                _isPlaying = true;
                foreach (var ps in _absorbParticles)
                {
                    ps.Play();
                }
                foreach (var tornado in _absorbTornadoes)
                {
                    tornado.StartEffect();
                }
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
                _isPlayingBlow = true;
                foreach (var ps in _rejectParticles)
                {
                    ps.Play();
                }
                foreach (var tornado in _rejectTornadoes)
                {
                    tornado.StartEffect();
                }
            }
        }
    }

    public void StopEffect()
    {
        if (_absorb)
        {
            foreach (var ps in _absorbParticles)
            {
                ps.Stop();
            }
            foreach (var tornado in _absorbTornadoes)
            {
                tornado.StopEffect();
            }
        }
        if (_blow) 
        {
            foreach (var ps in _rejectParticles)
            {
                ps.Stop();
            }
            foreach (var tornado in _rejectTornadoes)
            {
                tornado.StopEffect();
            }
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
