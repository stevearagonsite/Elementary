using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(AudioSource))]
public class AudioClipHandler : MonoBehaviour
{
    private AudioSource _source;
    private float _initialVolume;
    private void Start()
    {
        _source = GetComponent<AudioSource>();
        _initialVolume = _source.volume;
    }
    public void Play()
    {
        _source.PlayOneShot(_source.clip) ;
    }

    public void Stop()
    {
        _source.Stop();
    }

    public void PlayFadeIn(float transitionTime)
    {
        StopAllCoroutines();
        StartCoroutine(FadeInCoroutine(transitionTime));
    }

    private IEnumerator FadeInCoroutine(float transitionTime)
    {
        _source.Play();
        var time = 0f;
        while (time < transitionTime)
        {
            _source.volume = (time / transitionTime) * _initialVolume;
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
    
    public void StopFadeOut(float transitionTime)
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutCoroutine(transitionTime));
    }

    private IEnumerator FadeOutCoroutine(float transitionTime)
    {
        var time = 0f;
        while (1-time > 0)
        {
            _source.volume = (1 - time / transitionTime) * _initialVolume;
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        _source.Stop();
    }
}
