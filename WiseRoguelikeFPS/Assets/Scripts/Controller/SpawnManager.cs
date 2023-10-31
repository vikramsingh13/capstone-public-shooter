using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject objectivePrefab;
    public Terrain terrain;
    public float spawnMargin;

    private void Start()
    {
        SpawnPlayerAndObjective();
    }

    private void SpawnPlayerAndObjective()
    {
        float terrainWidth = terrain.terrainData.size.x;
        float terrainLength = terrain.terrainData.size.z;

        Vector3 playerSpawnPosition;
        do
        {
            playerSpawnPosition = new Vector3(
                Random.Range(spawnMargin, terrainWidth / 2 - spawnMargin),
                0,
                Random.Range(spawnMargin, terrainLength - spawnMargin)
            );
            playerSpawnPosition.y = terrain.SampleHeight(playerSpawnPosition) + playerPrefab.transform.localScale.y / 2 + 8f;
        } while (Physics.CheckSphere(playerSpawnPosition, playerPrefab.transform.localScale.x / 2));

        Vector3 objectiveSpawnPosition;
        do
        {
            objectiveSpawnPosition = new Vector3(
                Random.Range(terrainWidth / 2 + spawnMargin, terrainWidth - spawnMargin),
                0,
                Random.Range(spawnMargin, terrainLength - spawnMargin)
            );
            objectiveSpawnPosition.y = terrain.SampleHeight(objectiveSpawnPosition) + objectivePrefab.transform.localScale.y / 2 + 9f;
        } while (Physics.CheckSphere(objectiveSpawnPosition, objectivePrefab.transform.localScale.x / 2));

        Instantiate(playerPrefab, playerSpawnPosition, Quaternion.identity);
        Instantiate(objectivePrefab, objectiveSpawnPosition, Quaternion.identity);
    }

}