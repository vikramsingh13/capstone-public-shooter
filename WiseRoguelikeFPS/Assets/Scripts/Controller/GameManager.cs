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

    // Start is called before the first frame update
    void Start()
    {

    }
}