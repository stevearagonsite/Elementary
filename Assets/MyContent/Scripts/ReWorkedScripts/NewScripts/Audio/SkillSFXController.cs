using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioClipHandler))]
public class SkillSFXController : MonoBehaviour
{
    public AudioClip vacuumClip;
    public AudioClip fireClip;
    public AudioClip electricClip;
    [Range(0,1)]
    public float skillVolume;
    public float transitionTime;

    private AudioClipHandler _clipHandler;
    private AudioSource _source;

    private Dictionary<Skills.Skills, AudioClip> _audioDictionary;
    private bool _isPlaying;
    private void Start()
    {
        _audioDictionary = new Dictionary<Skills.Skills, AudioClip>();
        _audioDictionary.Add(Skills.Skills.VACCUM, vacuumClip);
        _audioDictionary.Add(Skills.Skills.FIRE, fireClip);
        _audioDictionary.Add(Skills.Skills.ELECTRICITY, electricClip);

        _source = GetComponent<AudioSource>();
        _clipHandler = GetComponent<AudioClipHandler>();
    }

    public void PlaySkillSFX(Skills.Skills actualSkill)
    {
        if (!_isPlaying)
        {
            _source.clip = _audioDictionary[actualSkill];
            _clipHandler.PlayFadeIn(transitionTime);
            _isPlaying = true;
        }
    }

    public void StopSkillSFX()
    {
        _clipHandler.StopFadeOut(transitionTime);
        _isPlaying = false;
    }
}
