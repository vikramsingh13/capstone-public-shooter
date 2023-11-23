using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using System;
using System.Threading.Tasks;

public class EnemySpawner : MonoBehaviour
{
    //prefab of enemy to spawn. Attach spawner class to each spawner object and
    //using the editor set the specific enemy prefab to spawn for each of them.
    [SerializeField] 
    private string _enemyDataAddress;
    [SerializeField]
    private bool _isBossSpawner = false;
    private bool _bossSpawned = false;

    //spawn interval
    [SerializeField]
    [Header("Spawn point offset from spawner object")]
    private float _spawnOffset = 1f;
    [SerializeField]
    private int _maxSpawnedUnits = 5;
    [SerializeField]
    private float _spawnDelay = 5f;

    //List to track all the spawned enemies of each spawner
    private List<GameObject> _spawnedEnemies = new List<GameObject>();

    //delay check
    private bool waitingForDelay = false;

    private DiContainer _diContainer;
    private Level1Manager _level1Manager;
    
    //try to store the enemy data in the spawner class to help with async loading issues
    private EnemyData _enemyData;

    //demo: for demo purposes only to avoid multithreading issues
    private bool _trackerCleared = false;


    [Inject]
    public void Construct(DiContainer diContainer, Level1Manager level1Manager)
    {
        _diContainer = diContainer;
        _level1Manager = level1Manager;
    }

    public void Init(string enemyDataAddress)
    {
        _enemyDataAddress = enemyDataAddress;
    }

    void Start()
    {
        //DEMO: spawners are not set properly, align them with the pre defined parents
        try
        {
            transform.position = transform.parent.position;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to set spawner position to parent position: {e}");
        }
    }
    // Update is called once per frame
    void Update()
    {
        //If the spawned enemy objects have been destroyed and the spawner is not waiting for the delay to end, start the delay
        if (!_isBossSpawner && _spawnedEnemies.Count < _maxSpawnedUnits && !waitingForDelay)
        {
            waitingForDelay = true;
            SpawnEnemy();
        }
        else if (_isBossSpawner && _spawnedEnemies.Count == 0 && !_bossSpawned && _level1Manager.AllLevelObjectivesCompleted)
        {
            waitingForDelay = true;
            _bossSpawned = true;
            SpawnEnemy();
        }

        //If max spawned units have been reached, check for null objects in the list and remove them
        if (_spawnedEnemies.Count == _maxSpawnedUnits)
        {
            //iterate through _spawnedEnemies and remove all the null objects
            _spawnedEnemies.RemoveAll(item => item == null);
            
        }

        //for demo purposes only to avoid multithreading issues
        if(!_trackerCleared)
        {
            //only clears the references to the enemies, not the enemies themselves
            ClearAllEnemies();
        }
    }

    //Spawn a new enemy object at the spawner's position
    private void SpawnEnemy()
    {
        //async load enemy data, DiContainer Init the enemy and add it to the spawned enemies list
        Addressables.LoadAssetAsync<EnemyData>(_enemyDataAddress).Completed += (handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                if( _enemyData == null)
                {
                    _enemyData = handle.Result;
                }

                //async load the enemy prefab from the enemy data
                Addressables.LoadAssetAsync<GameObject>(_enemyData.prefabAddressKey).Completed += (handle) =>
                {
                    if(handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        //randomize the spawn position
                        //random offset based on the spawn offset
                        float randomOffset = UnityEngine.Random.Range(-_spawnOffset, _spawnOffset);
                        //calculate the spawn position
                        Vector3 spawnPosition = transform.position + Vector3.up * randomOffset;
                        //make sure the enemies don't go below the spawner
                        spawnPosition.y = transform.position.y;

                        //instantiate the enemy prefab
                        GameObject enemyPrefab = handle.Result;
                        _diContainer.InstantiatePrefab(enemyPrefab, spawnPosition, Quaternion.identity, null);
                        Debug.Log($"Init called for {enemyPrefab.name} in spawner");
                        Debug.Log($"Enemy data : {_enemyData}");
                        enemyPrefab.GetComponent<Enemy>().Init(_enemyData, this);
                        //add the spawned enemy to the list
                        _spawnedEnemies.Add(enemyPrefab);
                        //start the spawn delay
                        StartCoroutine(DelaySpawn(_spawnDelay));
                    }
                    else
                    {
                        Debug.LogError($"Failed to load enemy data in EnemySpawner::SpawnEnemy for address: {_enemyDataAddress}");
                    }
                };
            }
            else
            {
                Debug.LogError($"Failed to load enemy data in EnemySpawner::SpawnEnemy for address: {_enemyDataAddress}");
            }
        };
    }

    // Method to be called when an enemy is removed
    public void EnemyRemoved(GameObject enemyToRemove)
    {
        try
        {
            if (_spawnedEnemies.Contains(enemyToRemove))
            {
                _spawnedEnemies.Remove(enemyToRemove);
            }
        }
        catch
        {
            //iter through _spawnedEnemies and remove all the null objects
            _spawnedEnemies.RemoveAll(item => item == null);
        }
    }

    //todo: refactor to use events.
    //demo: for demo purposes only to avoid multithreading issues
    private void ClearAllEnemies()
    {
        _spawnedEnemies = new List<GameObject>();
        _trackerCleared = true;
        StartCoroutine(DelayedTrackerClear());
    }

    private IEnumerator DelayedTrackerClear(float delay = 30)
    {
        yield return new WaitForSecondsRealtime(delay);
        _trackerCleared = false;
    }

    private IEnumerator DelaySpawn(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        //reset delay check
        waitingForDelay = false;
    }
}