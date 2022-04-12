using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public Sound[] sounds;

    public AudioMixerGroup group;

    public List<String> soundsToPlayOnAwake;
    private string _volumeParameter = "MasterVolume";
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
      
    }

    private void Start()
    {
        //DontDestroyOnLoad(gameObject);
        foreach (Sound s in sounds)
        {
            s.source = s.gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.spatialBlend = 1;
            s.source.outputAudioMixerGroup = group;
        }

        foreach (var sound in soundsToPlayOnAwake)
        {
            Play(sound);
        }
    }
    
    public void Play(string name) // Play a sound
    {
        Sound s = Array.Find(sounds, sound => sound.soundName == name);
        if (s != null && s.canPlay)
        {
            s.source.Play();
        }
    }

    public void Stop(string name) // Stop a sound
    {
        Sound s = Array.Find(sounds, sound => sound.soundName == name);
        if (s != null)
        {
            s.source.Stop();
        }
    }
    public void Pause(string name) // Stop a sound
    {
        Sound s = Array.Find(sounds, sound => sound.soundName == name);
        if (s != null)
        {
            s.source.Pause();
        }
    }
    public void UnPause(string name) // Stop a sound
    {
        Sound s = Array.Find(sounds, sound => sound.soundName == name);
        if (s != null && s.canPlay)
        {
            s.source.UnPause();
        }
    }
}
