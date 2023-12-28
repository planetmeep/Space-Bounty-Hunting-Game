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
    public float maxDistance;
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

    public Sound GetCurrentSound(string name) 
    {
        Sound currentSound = Array.Find(sounds, sound => sound.soundName == name);
        return currentSound;
    }

    public void PlaySound(string name) 
    {
        GetAudioSource(name).Play();
    }

    public void ResetPlaySound(string name)
    {
        GetAudioSource(name).Stop();
        GetAudioSource(name).Play();
    }
    public void StopSound(string name)
    {
        GetAudioSource(name).Stop();
    }

    public void PlayWithDistance(string name, Vector3 position) 
    {
        float distance = Vector3.Distance(transform.position, position);
        GetAudioSource(name).volume = Mathf.Lerp(GetCurrentSound(name).volume, 0, distance / maxDistance);
        GetAudioSource(name).Stop();
        PlaySound(name);
    }

    public void PlayImpactWithDistance(HitsoundMaterials material, Vector3 position)
    {
        string materialName = material.ToString();
        float distance = Vector3.Distance(transform.position, position);
        print(Mathf.Lerp(GetAudioSource(materialName + "Impact").volume, 0, distance / maxDistance));
        GetAudioSource(materialName + "Impact").volume = Mathf.Lerp(GetCurrentSound(materialName + "Impact").volume, 0, distance / maxDistance);
        GetAudioSource(materialName + "Impact").pitch = UnityEngine.Random.Range(0.8f, 1.2f);
        GetAudioSource(materialName + "Impact").Stop();
        GetAudioSource(materialName + "Impact").Play();
    }

    public void PlayImpactSound(HitsoundMaterials material) 
    {
        string materialName = material.ToString();
        GetAudioSource(materialName + "Impact").pitch = UnityEngine.Random.Range(0.8f, 1.2f);
        GetAudioSource(materialName + "Impact").Stop();
        GetAudioSource(materialName + "Impact").Play(); 
    }
}
