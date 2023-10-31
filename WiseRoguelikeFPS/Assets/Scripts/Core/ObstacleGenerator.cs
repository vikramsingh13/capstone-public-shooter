using UnityEngine;

public class ObstacleGenerator : MonoBehaviour
{
    public GameObject[] obstaclePrefabs; // An array of obstacle prefabs to randomly generate
    public Terrain terrain;             // The terrain on which the obstacles will be generated
    public int obstacleCount;           // The number of obstacles to generate
    public float minScale, maxScale;    // The minimum and maximum scale of the generated obstacles
    public float noiseScale;            // The scale of the Perlin noise used to determine obstacle placement
    public float noiseThreshold;        // The threshold value for the Perlin noise used to determine obstacle placement

    private void Start()
    {
        GenerateObstacles();
    }

    private void GenerateObstacles()
    {
        float margin = 100f;            // The margin from the edges of the terrain
        float lowerOffset = 0.8f;       // The offset to place obstacles slightly below the terrain surface

        for (int i = 0; i < obstacleCount; i++)
        {
            // Generate a random position for the obstacle within the terrain boundaries
            Vector3 randomPosition = new Vector3(
                Random.Range(margin, terrain.terrainData.size.x - margin),
                -1,
                Random.Range(margin, terrain.terrainData.size.z - margin)
            );

            // Set the y-axis position of the obstacle to the height of the terrain at the random position, adjusted by the lower offset
            randomPosition.y = terrain.SampleHeight(randomPosition) + terrain.transform.position.y - lowerOffset;

            // Generate a Perlin noise value at the random position and check if it is above the noise threshold
            float noiseValue = Mathf.PerlinNoise(randomPosition.x * noiseScale, randomPosition.z * noiseScale);
            if (noiseValue > noiseThreshold)
            {
                // Select a random obstacle prefab from the obstacle prefabs array
                GameObject obstaclePrefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
                
                // Instantiate the selected obstacle prefab at the random position with no rotation
                GameObject obstacle = Instantiate(obstaclePrefab, randomPosition, Quaternion.identity);

                // Randomize the scale of the obstacle within the specified range
                obstacle.transform.localScale *= Random.Range(minScale, maxScale);

                // Set the parent of the obstacle to the parent transform of this script (useful for organizing objects in the hierarchy)
                obstacle.transform.SetParent(transform);
            }
        }
    }
}
