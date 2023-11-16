using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class AudioManager : Singleton<AudioManager>
{
    private AudioSource _musicSource;
    private bool _isMusicPaused;
    private AudioSource _ambienceSource;
    private bool _isAmbiencePaused;

    [SerializeField]
    private float _musicVolume = 1f;
    private float _ambienceVolume = 1f;
    private float _effectsVolume = 1f;
    //static event other scripts can subscribe to when music volume is changed
    public delegate void MusicVolumeChangedEvent(float volume);
    public delegate void EffectsVolumeChangedEvent(float volume);
    public delegate void AmbienceVolumeChangedEvent(float volume);

    [Inject]
    public void Construct()
    {    

    }

    private void Start()
    {
        //Get the audio sources from the main camera at start
        GetAudioSourcesFromMainCamera();

        //handle the volume change events
        Menu.MusicVolumeChangedEvent += HandleMusicVolumeChanged;
        Menu.AmbienceVolumeChangedEvent += HandleAmbienceVolumeChanged;
        Menu.EffectsVolumeChangedEvent += HandleEffectsVolumeChanged;


    }

    void Update()
    {
        if( _musicSource == null || _ambienceSource == null)
        {
            GetAudioSourcesFromMainCamera();
        }
    }

    private void GetAudioSourcesFromMainCamera()
    {
        //Get the audio sources from the main camera
        _musicSource = Camera.main.transform.Find("MusicSource").gameObject.GetComponent<AudioSource>();
        _ambienceSource = Camera.main.transform.Find("AmbienceSource").gameObject.GetComponent<AudioSource>();
    }

    //Music Related Properites
    public void StartMusic(AudioClip music)
    {
        _musicSource.Stop();
        _musicSource.PlayOneShot(music);
    }

    public void ToggleMusic()
    {
        _isMusicPaused = !_isMusicPaused;

        if(_isMusicPaused)
        {
            _musicSource.Pause();
        }
        else
        {
            _musicSource.UnPause();
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

        _musicVolume = volume;
        _musicSource.volume = _musicVolume;
    }

    //Ambience Related Properties
    public void StartAmbience()
    {
        _ambienceSource.Play();
    }

    public void ToggleAmbience()
    {

        _isAmbiencePaused = !_isAmbiencePaused;

        if (_isAmbiencePaused)
        {
            _ambienceSource.Pause();
        }
        else
        {
            _ambienceSource.UnPause();
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

        _ambienceVolume = volume;
        _ambienceSource.volume = _ambienceVolume;
    }

    //Effects Related Properties
    public void SetEffectsVolume(float volume)
    {

        if (volume < 0.0f)
        {
            volume = 0.0f;
        }

        if (volume > 1.0f)
        {
            volume = 1.0f;
        }
        _effectsVolume = volume;
    }

    //Invoked by a gameObject with its own effect source and clip.
    public void PlayEffectAudioClip(AudioSource effectSource, AudioClip audioClip)
    {
        //Plays the effect from the effectSource of the object calling it, with the volume adjusted by this script.
        effectSource.PlayOneShot(audioClip, _effectsVolume);
    }


    //Event Handlers
    public void HandleMusicVolumeChanged(float volume)
    {
        Debug.Log("Music Volume Changed to " + volume);
        SetMusicVolume(volume);
    }

    public void HandleAmbienceVolumeChanged(float volume)
    {
        SetAmbienceVolume(volume);
    }

    public void HandleEffectsVolumeChanged(float volume)
    {
        SetEffectsVolume(volume);
    }
}
