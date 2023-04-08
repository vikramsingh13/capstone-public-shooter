using UnityEngine;

public class ObstacleGenerator : MonoBehaviour
{
    public GameObject[] obstaclePrefabs;
    public Terrain terrain;
    public int obstacleCount;
    public float minScale, maxScale;
    public float noiseScale;
    public float noiseThreshold;

    private void Start()
    {
        GenerateObstacles();
    }

    private void GenerateObstacles()
    {
        float margin = 100f;

        for (int i = 0; i < obstacleCount; i++)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(margin, terrain.terrainData.size.x - margin),
                0,
                Random.Range(margin, terrain.terrainData.size.z - margin)
            );

            randomPosition.y = terrain.SampleHeight(randomPosition) + terrain.transform.position.y;

            float noiseValue = Mathf.PerlinNoise(randomPosition.x * noiseScale, randomPosition.z * noiseScale);
            if (noiseValue > noiseThreshold)
            {
                GameObject obstaclePrefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
                GameObject obstacle = Instantiate(obstaclePrefab, randomPosition, Quaternion.identity);
                
                Debug.Log("Instantiated obstacle: " + obstacle.name);
                obstacle.transform.localScale *= Random.Range(minScale, maxScale);
                obstacle.transform.SetParent(transform);
            }
        }
    }

}
