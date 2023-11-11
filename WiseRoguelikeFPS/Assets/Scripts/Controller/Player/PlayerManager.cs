using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
public class PlayerManager : Singleton<PlayerManager>
{

    //public Transform testSlope;

    public const float BASE_SPRINTSPEEDMOD = 1.5f;
    public const float BASE_CROUCHSPEEDMOD = 0.75f;

    public const float BASE_HEALTH = 100f;

    //Contollers
    private GameManager _gameManager;

    [SerializeField]
    private GameObject _player;
    public virtual GameObject GetPlayer
    {
        get { return _player; }
    }

    [Inject]
    public void Construct(GameManager gameManager)
    {
        Debug.Log("_playerManager ++++ init: ");
        _gameManager = gameManager;
        Debug.Log("after play manager Game: ++++: " + _gameManager);
    }

    // Start is called before the first frame update
    void Start()
    {
        FindPlayerObjectOrSpawnIt();
    }

    // Update is called once per frame
    void Update()
    {
        //no player actions are allowed when game is paused
        if( _gameManager != null && _gameManager.IsPaused)
        {
            if( _player == null)
            {
                FindPlayerObjectOrSpawnIt();
            }
        }
    }

    //finds the ref to the player object with the 'Player' tag.
    //If no player object is found in the current scene,
    //instantiates a new player object in the scene with player data
    private void FindPlayerObjectOrSpawnIt()
    {
        Debug.Log("Getting player now");
        if( _player == null ) 
        {
            _player = GameObject.FindWithTag("Player");
        }
        else
        {
            //TODO: init player prefab into the scene with player data
        }

        //if _player is still null, error log it.
        //this can be rewritten with try catch
        if( _player == null ) 
        {
            Debug.LogError("Failed to find Player or failed to init Player in PlayerManager::FindPlayerObjectOrSpawnIt.");
        }
    }
}
