using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    private static AudioManager _instance;
    public static AudioManager instance { get{ return _instance; } }

    public float musicVolume { get => _musicVolume; set => _musicVolume = Mathf.Clamp01(value); }
    public float masterVolume { get => _masterVolume; set => _masterVolume = Mathf.Clamp01(value); }
    public float sfxVolume { get => _sfxVolume; set => _sfxVolume = Mathf.Clamp01(value); }
    public float ambientVolume { get => _ambientVolume; set => _ambientVolume = Mathf.Clamp01(value); }

    private AudioSource _musicSource;
    private AudioSource _musicSource2;
    private AudioSource _sfxSource;
    private AudioSource _ambientSource;
    private AudioSource _ambientSource2;

    private bool _isFirstMusicSourcePlaying;
    private bool _isFirstAmbientSourcePlaying;

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

        _musicSource = this.gameObject.AddComponent<AudioSource>();
        _musicSource2 = this.gameObject.AddComponent<AudioSource>();
        _sfxSource = this.gameObject.AddComponent<AudioSource>();
        _ambientSource = this.gameObject.AddComponent<AudioSource>();
        _ambientSource2 = this.gameObject.AddComponent<AudioSource>();


        _musicSource.loop = true;
        _musicSource2.loop = true;
        _ambientSource.loop = true;
        _ambientSource2.loop = true;

    }
    #region Music
    public void PlayMusic(AudioClip clip, float volume = 1)
    {
        volume = Mathf.Clamp01(volume);
        var actualSource = _isFirstMusicSourcePlaying ? _musicSource2 : _musicSource;
        actualSource.clip = clip;
        actualSource.volume = _musicVolume * _masterVolume * volume;
        actualSource.Play();
    }

    public void PlayMusicWithFade(AudioClip newClip, float transitionTime = 1f, float volume = 1)
    {
        var actualSource = _isFirstMusicSourcePlaying ? _musicSource2 : _musicSource;
        StartCoroutine(UpdateMusicWithFade(actualSource, newClip, transitionTime, volume));
    }

    public void PlayMusicWithCrossFade(AudioClip newClip, float transitionTime = 1f, float volume = 1)
    {
        volume = Mathf.Clamp01(volume);
        var actualSource = _isFirstMusicSourcePlaying ? _musicSource2 : _musicSource;
        var newSource = _isFirstMusicSourcePlaying ? _musicSource : _musicSource2;

        _isFirstMusicSourcePlaying = !_isFirstMusicSourcePlaying;

        newSource.clip = newClip;
        newSource.Play();

        StartCoroutine(UpdateMusicWithCrossFade(actualSource, newSource, transitionTime, volume));
    }

    public void StopMusicWithFade(float transitionTime)
    {
        var actualSource = _isFirstMusicSourcePlaying ? _musicSource2 : _musicSource;
        StartCoroutine(UpdateMusicStopWithFade(actualSource, transitionTime));
    }

    private IEnumerator UpdateMusicStopWithFade(AudioSource actualSource, float transitionTime)
    {
        var t = 0f;
        var actualVolume = actualSource.volume;
        while (t < transitionTime)
        {
            actualSource.volume = _musicVolume * _masterVolume * (1 - t / transitionTime) * actualVolume;
            yield return null;
        }

        actualSource.Stop();
    }

    private IEnumerator UpdateMusicWithCrossFade(AudioSource actualSource, AudioSource newSource, float transitionTime, float volume)
    {
        var t = 0f;
        var actualVolume = actualSource.volume;
        while (t < transitionTime)
        {
            newSource.volume = _musicVolume * _masterVolume * (t / transitionTime) * volume;
            actualSource.volume = _musicVolume * _masterVolume * (1 - t / transitionTime) * actualVolume;
            yield return null;
        }

        actualSource.Stop();
    }

    private IEnumerator UpdateMusicWithFade(AudioSource source, AudioClip newClip, float transitionTime, float volume)
    {
        if (!source.isPlaying)
        {
            source.Play();
        }
        var t = 0f;
        var actualVolume = source.volume;
        while (t< transitionTime)
        {
            t += Time.deltaTime;
            source.volume = (_musicVolume * _masterVolume * actualVolume) * (1 - t/transitionTime);
            yield return null;
        }

        source.Stop();
        source.clip = newClip;
        source.Play();

        t = 0;
        while (t < transitionTime)
        {
            t += Time.deltaTime;
            source.volume = (_musicVolume * _masterVolume * volume) * (t / transitionTime);
            yield return null;
        }

    }
    #endregion

    #region ambient
    public void PlayAmbient(AudioClip clip, float volume = 1)
    {
        var actualSource = _isFirstAmbientSourcePlaying ? _ambientSource2 : _ambientSource;
        actualSource.clip = clip;
        actualSource.volume = _ambientVolume * _masterVolume * volume;
        actualSource.Play();
    }

    public void PlayAmbientWithFade(AudioClip newClip, float transitionTime = 1f, float volume = 1)
    {
        var actualSource = _isFirstAmbientSourcePlaying ? _ambientSource2 : _ambientSource;
        StartCoroutine(UpdateAmbientWithFade(actualSource, newClip, transitionTime, volume));
    }

    public void PlayAmbientWithCrossFade(AudioClip newClip, float transitionTime = 1f, float volume = 1)
    {
        var actualSource = _isFirstAmbientSourcePlaying ? _ambientSource2 : _ambientSource;
        var newSource = _isFirstAmbientSourcePlaying ? _ambientSource : _ambientSource2;

        _isFirstAmbientSourcePlaying = !_isFirstAmbientSourcePlaying;

        newSource.clip = newClip;
        newSource.Play();

        StartCoroutine(UpdateAmbientWithCrossFade(actualSource, newSource, transitionTime, volume));
    }

    public void StopAmbientWithFade(float transitionTime)
    {
        var actualSource = _isFirstMusicSourcePlaying ? _musicSource2 : _musicSource;
        StartCoroutine(UpdateAmbientStopWithFade(actualSource, transitionTime));
    }

    private IEnumerator UpdateAmbientStopWithFade(AudioSource actualSource, float transitionTime)
    {
        var t = 0f;
        var actualVolume = actualSource.volume;
        while (t < transitionTime)
        {
            actualSource.volume = _ambientVolume * _masterVolume * (1 - t / transitionTime) * actualVolume;
            yield return null;
        }

        actualSource.Stop();
    }

    private IEnumerator UpdateAmbientWithCrossFade(AudioSource actualSource, AudioSource newSource, float transitionTime, float volume)
    {
        var t = 0f;
        var actualVolume = actualSource.volume;
        while (t < transitionTime)
        {
            newSource.volume = _ambientVolume * _masterVolume * (t / transitionTime) * volume;
            actualSource.volume = _ambientVolume * _masterVolume * (1 - t / transitionTime) * actualVolume;
            yield return null;
        }

        actualSource.Stop();
    }

    private IEnumerator UpdateAmbientWithFade(AudioSource source, AudioClip newClip, float transitionTime, float volume)
    {
        if (!source.isPlaying)
        {
            source.Play();
        }
        var t = 0f;
        var actualVolume = source.volume;
        while (t < transitionTime)
        {
            t += Time.deltaTime;
            source.volume = (_ambientVolume * _masterVolume) * (1 - t / transitionTime) * actualVolume;
            yield return null;
        }

        source.Stop();
        source.clip = newClip;
        source.Play();

        t = 0;
        while (t < transitionTime)
        {
            t += Time.deltaTime;
            source.volume = (_ambientVolume * _masterVolume) * (t / transitionTime) * volume;
            yield return null;
        }

    }
    #endregion

    #region SFX
    public void PlaySFX(AudioClip clip)
    {
        _sfxSource.PlayOneShot(clip, _sfxVolume * _masterVolume);
    }

    public void PlaySFX(AudioClip clip, float vol)
    {
        vol = Mathf.Clamp01(vol);
        _sfxSource.PlayOneShot(clip, _sfxVolume * _masterVolume * vol);
    }
    #endregion


}
