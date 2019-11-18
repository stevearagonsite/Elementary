using System.Collections;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    static AudioManager _instance;
    public static AudioManager instance { get{ return _instance; } }

    public Audio[] sounds;

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

    void Start ()
    {
        DontDestroyOnLoad(gameObject);

        for (int i = 0; i < sounds.Length; i++)
        {
            var go = new GameObject("Sound_" + sounds[i].name);
            sounds[i].source = go.AddComponent<AudioSource>();
            if(sounds[i].parent != null)
                go.transform.SetParent(sounds[i].parent);

            sounds[i].source.clip = sounds[i].clip;
            sounds[i].source.volume = sounds[i].volume;
            sounds[i].source.pitch = sounds[i].pitch;
            sounds[i].source.loop = sounds[i].loop;
        }
    }
	
    public void Play(string name)
    {
        var s = Array.Find(sounds, sound => sound.name == name);
        s.source.Play();
    }
	
}
