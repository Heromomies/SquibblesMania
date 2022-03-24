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

    public Slider slider;
    public AudioMixer mixer;

    public List<String> soundsToPlayOnAwake;
    private string _volumeParameter = "MasterVolume";
    void Awake()
    {
        mixer.SetFloat(_volumeParameter,PlayerPrefs.GetFloat("Volume"));
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        slider.onValueChanged.AddListener(HandleSliderValueChanged);
    }

    public void HandleSliderValueChanged(float value) // When we change the value of the slider
    { 
        value  = slider.value;
        mixer.SetFloat(_volumeParameter, value);
        PlayerPrefs.SetFloat("Volume",value);
    }
    private void Start()
    {
        PlayerPrefs.GetFloat("Volume", slider.value);
       //DontDestroyOnLoad(gameObject);
        foreach (Sound s in sounds)
        {
            s.source = s.gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.spatialBlend = 1;
        }

        foreach (var sound in soundsToPlayOnAwake)
        {
            Play(sound);
        }

        foreach (var sound in sounds)
        {
            sound.GetComponent<AudioSource>().outputAudioMixerGroup = group;
        }
    }
    
    public void Play(string name) // Play a sound
    {
        Sound s = Array.Find(sounds, sound => sound.soundName == name);
        if (s != null)
        {
            s.source.Play();
        }
        else
        {
            Debug.LogWarning("Le son n'a pas été trouvé");
        }
        //Debug.Log("play");
    }

    public void Stop(string name) // Stop a sound
    {
        Sound s = Array.Find(sounds, sound => sound.soundName == name);
        if (s != null)
        {
            s.source.Stop();
        }
        else
        {
            Debug.LogWarning("Le son n'a pas été trouvé");
        }
    }
}
