using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    //prefab of enemy to spawn. Attach spawner class to each spawner object and
    //using the editor set the specific enemy prefab to spawn for each of them.
    [SerializeField] 
    private GameObject enemyPrefab;

    //spawn interval
    public float spawnOffset = 1f;
    public int maxSpawnedUnits = 5;

    //List to track all the spawned enemies of each spawner
    private List<GameObject> spawnedEnemies = new List<GameObject>();

    //delay check
    private bool waitingForDelay = false;

    // Update is called once per frame
    void Update()
    {
        //If the spawned enemy objects have been destroyed and the spawner is not waiting for the delay to end, start the delay
        if (spawnedEnemies.Count < maxSpawnedUnits && !waitingForDelay)
        {
            waitingForDelay = true;
            Invoke("SpawnEnemy", 5f);
        }
    }

    //Spawn a new enemy object at the spawner's position
    private void SpawnEnemy()
    {
        Vector3 spawnPosition = transform.position + Vector3.up * spawnOffset;

        //Instantiate a new enemy object at the spawn position with no rotation
        GameObject spawnedEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        //keep track of enemies spawned by each spawner
        spawnedEnemies.Add(spawnedEnemy);

        //reset waiting
        waitingForDelay = false;

        //event subscription to enemy's death event
        EnemyHealth enemyHealth = spawnedEnemy.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.OnDeath += (spawnedEnemy) => EnemyRemoved(spawnedEnemy);
        }
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