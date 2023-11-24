using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
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
        _musicSource = GameObject.FindWithTag("MusicSource")?.GetComponent<AudioSource>();
        _ambienceSource = GameObject.FindWithTag("AmbienceSource")?.GetComponent<AudioSource>();
        Debug.Log("Audio Sources found: " + _musicSource + " " + _ambienceSource);
    }

    //Music Related Properites
    public IEnumerator LoadAndPlayMusic(string musicAddress)
    {
        AsyncOperationHandle<AudioClip> handle = Addressables.LoadAssetAsync<AudioClip>(musicAddress);

        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            AudioClip music = handle.Result;
            _musicSource.Stop();
            _musicSource.PlayOneShot(music);
        }
        else
        {
            Debug.LogError($"Failed to load music in AudioManager::LoadAndPlayMusic for address: {musicAddress}");
        }

        // Release the handle when you're done to avoid resource leaks.
        Addressables.Release(handle);
    }
    
    public void StartMusic(string musicAddress)
    {
        _musicSource.Stop();
        StartCoroutine(WaitForThreeBeforePlayingMusic(musicAddress));
    }

    //Demo: hack to avoid no merge mode errors
    public IEnumerator WaitForThreeBeforePlayingMusic(string musicAddress, float seconds = 3f)
    {
        yield return new WaitForSecondsRealtime(seconds);
        StartCoroutine(LoadAndPlayMusic(musicAddress));
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
