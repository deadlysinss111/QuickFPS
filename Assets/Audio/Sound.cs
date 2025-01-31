using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public string name;
    
    public AudioClip clip;
    public AudioMixerGroup output;
    public bool playOnStart = false;

    [Range(0, 1)]
    public float volume = 0.5f;
    [Range(.1f, 3)]
    public float pitch = 1;

    public bool loop;

    [HideInInspector] public AudioSource source;

}
