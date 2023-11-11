using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    private bool _isPaused;
    public virtual bool IsPaused
    {
        get { return _isPaused; }
        set { _isPaused = value; }
    }
    private AudioManager _audioManager;
    public virtual AudioManager GetAudioManager
    {
        get { return _audioManager; }
    }

    [Inject]
    public void Construct(AudioManager audioManager)
    {
        _audioManager = audioManager;
    }

    // Start is called before the first frame update
    void Start()
    {

    }
}