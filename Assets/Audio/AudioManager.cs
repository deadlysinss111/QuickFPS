using System;
using System.Collections;
using UnityEngine;


public class AudioManager : MonoBehaviour
{

    public Sound[] sounds;
    private string _default;

    public void Init()
    {
        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();

            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
            sound.source.outputAudioMixerGroup = sound.output;
            if(sound.playOnStart)
                Play(sound.name);
        }
    }

    public void Play(string name)
    {
        Sound sound = Array.Find(sounds, sound => sound.name == name);

        if (sound == null)
        {
            Debug.LogWarning($"Sound: {name} not found!");
            return;
        }

        sound.source.Play();
    }

    public void PlayDefault()
    {
        Play(_default);
    }

    public void FadeOutAllSources(float time)
    {
        foreach(Sound sound in sounds)
        {
            StartCoroutine(FadeOutTarget(sound.source, time));
        }
    }


    public static IEnumerator FadeOutTarget(AudioSource audioSource, float FadeTime)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;

            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }

}
