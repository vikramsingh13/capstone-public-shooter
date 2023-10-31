using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    public GameObject[] spawners; // Array of spawner prefabs
    public Terrain terrain; // Reference to the terrain object
    public int spawnerCount; // Number of spawners to generate
    public float minScale, maxScale; // Min and max scales for spawners
    public float noiseScale; // Scale factor for Perlin noise
    public float noiseThreshold; // Threshold for spawning based on Perlin noise

    private void OnDrawGizmosSelected()
    {
        float margin = 150f; // Margin to keep spawners away from the edges of the terrain

        // Draw a wireframe cube around the spawn area
        Vector3 cubeCenter = new Vector3(
            terrain.terrainData.size.x / 2f,
            0,
            terrain.terrainData.size.z / 2f
        );
        Vector3 cubeSize = new Vector3(
            terrain.terrainData.size.x - margin * 2f,
            100f,
            terrain.terrainData.size.z - margin * 2f
        );
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(cubeCenter + terrain.transform.position, cubeSize);
    }

    private void Start()
    {
        GenerateObstacles(); // Call GenerateObstacles on start
    }

    private void GenerateObstacles()
    {
        float margin = 150f; // Margin to keep spawners away from the edges of the terrain

        // Loop to generate spawners
        for (int i = 0; i < spawnerCount; i++)
        {
            // Generate a random position on the terrain, respecting the margin
            Vector3 randomPosition = new Vector3(
                Random.Range(margin, terrain.terrainData.size.x - margin),
                0,
                Random.Range(margin, terrain.terrainData.size.z - margin)
            );

            // Set the Y position based on terrain height at that position
            randomPosition.y = terrain.SampleHeight(randomPosition) + terrain.transform.position.y;

            // Calculate Perlin noise value at the random position
            float noiseValue = Mathf.PerlinNoise(randomPosition.x * noiseScale, randomPosition.z * noiseScale);

            // Check if the noise value is above the threshold
            if (noiseValue > noiseThreshold)
            {
                // Choose a random spawner prefab
                GameObject spawnerPrefab = spawners[Random.Range(0, spawners.Length)];

                // Instantiate the spawner at the random position
                GameObject spawner = Instantiate(spawnerPrefab, randomPosition, Quaternion.identity);

                // Randomly scale the spawner
                spawner.transform.localScale *= Random.Range(minScale, maxScale);

                // Set the spawner as a child of this object
                spawner.transform.SetParent(transform);

                // Get a valid position on the NavMesh for the spawned enemy
                UnityEngine.AI.NavMeshHit navMeshHit;
                if (UnityEngine.AI.NavMesh.SamplePosition(spawner.transform.position, out navMeshHit, 100f, UnityEngine.AI.NavMesh.AllAreas))
                {
                    // Set the position of the spawned enemy to the valid NavMesh position
                    spawner.transform.position = navMeshHit.position;
                }
            }
        }
    }

}