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

    //spawn interval
    [SerializeField]
    [Header("Spawn point offset from spawner object")]
    private float _spawnOffset = 1f;
    [SerializeField]
    private int _maxSpawnedUnits = 5;
    [SerializeField]
    private float _spawnDelay = 5f;

    //List to track all the spawned enemies of each spawner
    private List<GameObject> spawnedEnemies = new List<GameObject>();

    //delay check
    private bool waitingForDelay = false;

    DiContainer _diContainer;

    [Inject]
    public void Construct(DiContainer diContainer)
    {
        _diContainer = diContainer;
    }

    public void Init(string enemyDataAddress)
    {
        _enemyDataAddress = enemyDataAddress;
    }
    // Update is called once per frame
    void Update()
    {
        //If the spawned enemy objects have been destroyed and the spawner is not waiting for the delay to end, start the delay
        if (spawnedEnemies.Count < _maxSpawnedUnits && !waitingForDelay)
        {
            waitingForDelay = true;
            Invoke("SpawnEnemy", _spawnDelay);
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
                        enemyPrefab.GetComponent<Enemy>().Init(enemyData);

                        //reset delay check
                        waitingForDelay = false;
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
        if (spawnedEnemies.Contains(enemyToRemove))
        {
            spawnedEnemies.Remove(enemyToRemove);
        }
    }
}