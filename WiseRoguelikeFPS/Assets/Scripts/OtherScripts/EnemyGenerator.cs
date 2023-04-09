using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    public GameObject[] spanwer;
    public Terrain terrain;
    public int spawnerCount;
    public float minScale, maxScale;
    public float noiseScale;
    public float noiseThreshold;

    private void Start()
    {
        GenerateObstacles();
    }

    private void GenerateObstacles()
    {
        float margin = 150f;

        for (int i = 0; i < spawnerCount; i++)
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
                GameObject obstaclePrefab = spanwer[Random.Range(0, spanwer.Length)];
                GameObject obstacle = Instantiate(obstaclePrefab, randomPosition, Quaternion.identity);
                
                Debug.Log("Instantiated obstacle: " + obstacle.name);
                obstacle.transform.localScale *= Random.Range(minScale, maxScale);
                obstacle.transform.SetParent(transform);
            }
        }
    }

}
