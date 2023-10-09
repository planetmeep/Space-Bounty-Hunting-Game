using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public Sound[] songs;

    public bool playMusicOnStart;

    // Singleton
    public static AudioManager instance = null;
    public int startingSongIndex = 0;

    private void Awake()
    {
        if (instance == null) 
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        CreateAudioSources(sounds);
        CreateAudioSources(songs);
    }

    private void Start()
    {
        if (playMusicOnStart) 
        {
            songs[startingSongIndex].audioSource.Play();
        }
    }

    public void CreateAudioSources(Sound[] sounds)
    {
        foreach (Sound s in sounds)
        {
            s.audioSource = gameObject.AddComponent<AudioSource>();
            s.audioSource.clip = s.audioClip;
            s.audioSource.volume = s.volume;
            s.audioSource.pitch = s.pitch;
            s.audioSource.loop = s.loop;
        }
    }

    // Find AudioSource of sound with name
    public AudioSource GetAudioSource(string name)
    {
        Sound currentSound = Array.Find(sounds, sound => sound.soundName == name);
        if (currentSound == null)
        {
            Debug.LogError("Sound \"" + name + "\" Does not Exist in Audio Manager");
            return null;
        }
        return currentSound.audioSource;
    }
    public void PlaySound(string name) 
    {
        GetAudioSource(name).Play();
    }

    public void StopSound(string name) 
    {
        GetAudioSource(name).Stop();
    }
}
