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

    public Slider sliderMainSound;
    public Slider sliderMusicSound;
    public Slider sliderSfxSound;
    public AudioMixer mixer;

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
        sliderMainSound.onValueChanged.AddListener(HandleSliderMainValueChanged);
    }

    private void HandleSliderMainValueChanged(float value) // When we change the value of the slider
    { 
        value  = sliderMainSound.value;
      
        mixer.SetFloat(_volumeParameter, Mathf.Log10(value) * 20);
    }
    
    private void HandleSliderMusicValueChanged(float value) // When we change the value of the slider
    { 
        value  = sliderMusicSound.value;
        mixer.outputAudioMixerGroup.audioMixer.SetFloat(_volumeParameter, Mathf.Log10(value) * 20);
    }
    
    private void HandleSliderSfxValueChanged(float value) // When we change the value of the slider
    { 
        value  = sliderSfxSound.value;
        mixer.SetFloat(_volumeParameter, Mathf.Log10(value) * 20);
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
        if (s != null)
        {
            s.source.Play();
        }
        else
        {
            Debug.LogWarning("Le son n'a pas été trouvé");
        }
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
