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

    public Slider sliderMainSound;
    public Toggle mainToggle, effectToggle;

    public int multiplier;
    
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
      
        mixer.SetFloat(_volumeParameter, Mathf.Log10(value) * multiplier);

        PlayerPrefs.SetFloat("Sound", value);
        
        Debug.Log(value);
    }
    
    private void Start()
    {
        if (PlayerPrefs.HasKey("Sound"))
        {
            sliderMainSound.value = PlayerPrefs.GetFloat("Sound");
        }
        
        //DontDestroyOnLoad(gameObject);
        foreach (Sound s in sounds)
        {
            s.source = s.gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            
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
    private void Pause(string name) // Pause a sound
    {
        Sound s = Array.Find(sounds, sound => sound.soundName == name);
        if (s != null)
        {
            s.source.Pause();
        }
    }
    private void UnPause(string name) // UnPause a sound
    {
        Sound s = Array.Find(sounds, sound => sound.soundName == name);
        if (s != null && s.canPlay)
        {
            s.source.UnPause();
        }
    }
    
    public void StopMainMusic()
    {
        if (mainToggle.isOn)
        {
            UnPause("MainSound");
        }
        else
        {
            Pause("MainSound");
        }
    }
    
    public void StopEffectMusic()
    {
        if (effectToggle.isOn)
        {
            foreach (var s in sounds)
            {
                if(s.isEffect)
                    s.canPlay = true;
            }
        }
        else
        {
            foreach (var s in sounds)
            {
                if(s.isEffect)
                    s.canPlay = false;
            }
        }
    }

}
