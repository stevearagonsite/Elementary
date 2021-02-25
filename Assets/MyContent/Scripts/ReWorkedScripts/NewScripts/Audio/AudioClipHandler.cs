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
        EventManager.AddEventListener(GameEvent.STOP_ALL_SONUNDS, StopOnEvent);
    }
    public void Play()
    {
        _source.PlayOneShot(_source.clip) ;
    }

    public void Stop()
    {
        _source.Stop();
    }

    public void StopOnEvent(object[] p = null)
    {
        StopFadeOut(1);
    }

    public void FadeToVolume(float volume)
    {
        StopAllCoroutines();
        volume = Mathf.Clamp01(volume);
        if(volume != _source.volume)
        {
            StartCoroutine(FadeToVolumeCorroutine(volume));
        }
    }

    private IEnumerator FadeToVolumeCorroutine(float volume)
    {
        while(_source.volume > volume)
        {
            _source.volume -= Time.deltaTime;
            Debug.Log("Source Volume: " + _source.volume + ", volume: " + volume);
            yield return null;
        }

        while (_source.volume < volume)
        {
            _source.volume += Time.deltaTime;
            yield return null;
        }
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

    private void OnDestroy()
    {
        EventManager.RemoveEventListener(GameEvent.STOP_ALL_SONUNDS, StopOnEvent);
    }
}
