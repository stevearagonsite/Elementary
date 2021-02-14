using System.Collections;
using UnityEngine;
using UnityEngine.Audio;


public class AudioManager : MonoBehaviour {

    private static AudioManager _instance;
    public static AudioManager instance { get{ return _instance; } }

    public float musicVolume { 
        get => _musicVolume;
        set {
            mixer.SetFloat("MusicVolume", Mathf.Clamp01(value));
            _musicVolume = value;
        } 
    }
    public float masterVolume { 
        get => _masterVolume;
        set {
            mixer.SetFloat("MasterVolume", Mathf.Clamp01(value));
            _masterVolume = value;
        } 
    }
    public float sfxVolume { 
        get => _sfxVolume;
        set
        {
            mixer.SetFloat("SFXVolume", Mathf.Clamp01(value));
            sfxVolume = value;
        } 
    }
    public float ambientVolume { 
        get => _ambientVolume; 
        set {
            mixer.SetFloat("AmbientVolume", Mathf.Clamp01(value));
            _ambientVolume = value;
        }  
    }

    public AudioMixer mixer;

    private float _musicVolume = 1;
    private float _masterVolume = 1;
    private float _sfxVolume = 1;
    private float _ambientVolume = 1;

    void Awake()
    {
        if(_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }
}
public enum SoundType
{
    MUSIC,
    SFX,
    AMBIENT
}
