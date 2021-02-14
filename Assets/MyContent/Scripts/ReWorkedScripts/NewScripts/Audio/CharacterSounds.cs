using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioClipHandler))]
public class CharacterSounds : MonoBehaviour
{
    public FootStepSoundCheck fssc;

    public AudioClip[] stepMarmolClips;
    public AudioClip[] stepSandClips;
    public AudioClip[] stepWaterClips;
    public STEP_TYPE stepType;

    private AudioClipHandler _clipHandler;
    private AudioSource _source;
    private Dictionary<STEP_TYPE, AudioClip[]> stepDict;
    [Range(0,1)]
    public float stepVolume;
    // Start is called before the first frame update

    private void Start()
    {
        stepDict = new Dictionary<STEP_TYPE, AudioClip[]>();
        stepDict.Add(STEP_TYPE.SAND, stepSandClips);
        stepDict.Add(STEP_TYPE.STONE, stepMarmolClips);
        stepDict.Add(STEP_TYPE.WATER, stepWaterClips);
        _clipHandler = GetComponent<AudioClipHandler>();
        _source = GetComponent<AudioSource>();

    }
    public void Step()
    {
        stepType = fssc.currentType;
        var clips = stepDict[stepType];
        var number = Random.Range(0, clips.Length);
        //AudioManager.instance.PlaySFX(clips[number], stepVolume);
        _source.clip = clips[number];
        _clipHandler.Play();
    }

    public void Land()
    {
        stepType = fssc.ForceGetcurrentType();
        var clips = stepDict[stepType];
        var number = Random.Range(0, clips.Length);
        //AudioManager.instance.PlaySFX(clips[number], stepVolume);
        //AudioManager.instance.PlaySFX(clips[number], stepVolume);
        _source.clip = clips[number];
        _clipHandler.Play();
        _clipHandler.Play();
    }
}

public enum STEP_TYPE
{
    SAND,
    STONE,
    WATER
}
