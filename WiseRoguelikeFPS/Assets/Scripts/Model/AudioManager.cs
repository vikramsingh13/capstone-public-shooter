using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public static AudioManager instance;

    public AudioSource musicSource;

    public bool musicPaused;

    public AudioSource ambienceSource;

    public bool ambiencePaused;

    public float musicVolume = 0.5f;

    public float ambienceVolume = 0.5f;

    public float effectsVolume = 0.5f;


    private void Awake()
    {
        
        if(instance == null)
        {

            instance = this;

        }
        else
        {

            Destroy(gameObject);

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
