using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

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
    }

    //Spawn a new enemy object at the spawner's position
    private void SpawnEnemy()
    {
        Vector3 spawnPosition = transform.position + Vector3.up * _spawnOffset;

        //async load enemy data, DiContainer Init the enemy and add it to the spawned enemies list
        Addressables.LoadAssetAsync<EnemyData>(_enemyDataAddress).Completed += (handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                //async load the enemy prefab from the enemy data
                EnemyData enemyData = handle.Result;

                Addressables.LoadAssetAsync<GameObject>(enemyData.prefabAddressKey).Completed += (handle) =>
                {
                    if(handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        GameObject enemyPrefab = handle.Result;
                        _diContainer.InstantiatePrefab(enemyPrefab, spawnPosition, Quaternion.identity, null);
                        Debug.Log($"Init called for {enemyPrefab.name} in spawner");
                        enemyPrefab.GetComponent<Enemy>().Init(enemyData);
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

    // Method to be called when an enemy is removed by Combat Manager
    public void EnemyRemoved(GameObject enemyToRemove)
    {
        if (_spawnedEnemies.Contains(enemyToRemove))
        {
            _spawnedEnemies.Remove(enemyToRemove);
        }
    }

    private IEnumerator DelaySpawn(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        //reset delay check
        waitingForDelay = false;
    }
}