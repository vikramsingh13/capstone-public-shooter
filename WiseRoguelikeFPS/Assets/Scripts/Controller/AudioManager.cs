using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class AudioManager : Singleton<AudioManager>
{
    private AudioSource musicSource;

    private bool musicPaused;

    private AudioSource ambienceSource;

    private bool ambiencePaused;

    private float musicVolume = 0.5f;

    private float ambienceVolume = 0.5f;

    private float effectsVolume = 0.5f;

    [Inject]
    public void Construct()
    {

        Debug.Log("Starting up the AudioManager!");       

    }

    private void Start()
    {      
        
        musicSource = GameObject.Find("MusicSource").gameObject.GetComponent<AudioSource>();
        ambienceSource = GameObject.Find("AmbienceSource").gameObject.GetComponent<AudioSource>();

        if (musicSource != null)
        {

            Debug.Log("Found the music source!");

        }

        if (ambienceSource != null)
        {

            Debug.Log("Found the ambience source!");

        }

    }


    void Update()
    {

        musicSource.volume = musicVolume;
        ambienceSource.volume = ambienceVolume;
        
    }

    //Music Related Properites
    public void StartMusic(AudioClip music)
    {

        musicSource.Stop();
        musicSource.PlayOneShot(music);

    }

    public void MuteMusic()
    {

        musicVolume = 0.0f;

    }

    public void ToggleMusic()
    {

        musicPaused = !musicPaused;

        if(musicPaused)
        {

            musicSource.Pause();

        }
        else
        {

            musicSource.UnPause();

        }

    }

    public void SetMusicVolume(float volume)
    {

        if(volume < 0.0f)
        {

            volume = 0.0f;

        }

        if(volume > 1.0f) { 
        
            volume = 1.0f;
        
        }

        musicVolume = volume;

        musicSource.volume = musicVolume;

    }

    //Ambience Related Properties
    public void StartAmbience()
    {

        ambienceSource.Play();

    }

    public void MuteAmbience()
    {

        ambienceVolume = 0.0f;

    }

    public void ToggleAmbience()
    {

        ambiencePaused = !ambiencePaused;

        if (ambiencePaused)
        {

            ambienceSource.Pause();

        }
        else
        {

            ambienceSource.UnPause();

        }

    }

    public void SetAmbienceVolume(float volume)
    {

        if (volume < 0.0f)
        {

            volume = 0.0f;

        }

        if (volume > 1.0f)
        {

            volume = 1.0f;

        }

        ambienceVolume = volume;

        ambienceSource.volume = ambienceVolume;

    }

    //Sound Effect Related Properties
    public void SetSFXVolume(float volume)
    {

        if (volume < 0.0f)
        {

            volume = 0.0f;

        }

        if (volume > 1.0f)
        {

            volume = 1.0f;

        }

        effectsVolume = volume;

    }

    //Invoked by a gameObject with its own effect source and clip.
    public void PlaySFX(AudioSource effectSource, AudioClip audioClip)
    {

        //Plays the effect from the effectSource of the object calling it, with the volume adjusted by this script.
        effectSource.PlayOneShot(audioClip, effectsVolume);

    }

}
