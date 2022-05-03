using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Sound : MonoBehaviour
{
    public string soundName;

    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume;
    [Range(.1f,3f)]
    public float pitch;

    [HideInInspector]
    public AudioSource source;

    public bool loop;

    public bool canPlay;
    public bool isEffect;
}
