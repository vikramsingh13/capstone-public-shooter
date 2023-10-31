using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPlacement : MonoBehaviour
{
    public GameObject player;       // The player object to be randomly placed on the terrain
    public GameObject objective;    // The objective object to be placed on the opposite side of the terrain
    public Terrain terrain;         // The terrain on which the objects will be placed
    public float margin = 200.0f;   // Margin from the edges of the terrain
    public float edgeDistance = 10f; // Distance from the edge of the spawnable area

    // Start is called before the first frame update
    void Start()
    {
        // Get the size of the terrain
        Vector3 terrainSize = terrain.terrainData.size;

        // Generate a random position for the player object using the GetRandomPosition method
        Vector3 playerPosition = GetRandomPosition(terrainSize, player.GetComponent<Collider>().bounds.size.y);
        
        // Ensure the objective is placed on the opposite side of the terrain from the player object
        Vector3 objectivePosition = GetOppositePosition(terrainSize, playerPosition, objective.GetComponent<BoxCollider>().size.y);

        // Set the positions of both objects using the generated positions
        player.transform.position = playerPosition;
        objective.transform.position = objectivePosition;
    }

    // Generate a random position for an object within the terrain boundaries
    Vector3 GetRandomPosition(Vector3 terrainSize, float objectHeight)
    {
        // Generate random X and Z coordinates near the edges of the spawnable area
        float randomX = GetRandomEdgeCoordinate(margin, terrainSize.x - margin, edgeDistance);
        float randomZ = GetRandomEdgeCoordinate(margin, terrainSize.z - margin, edgeDistance);

        // Sample the height of the terrain at the random position and add half the object height plus a constant value of 10 units to the y-axis position
        float height = terrain.SampleHeight(new Vector3(randomX, 0, randomZ)) + objectHeight / 2 + 10f;

        // Return the random position vector
        return new Vector3(randomX, height, randomZ);
    }


    // Generate a position on the opposite side of the terrain from a given position
    Vector3 GetOppositePosition(Vector3 terrainSize, Vector3 playerPosition, float objectHeight)
    {
        // Calculate the X and Z coordinates for the opposite position by subtracting the player's X and Z coordinates from the terrain size
        float oppositeX = terrainSize.x - playerPosition.x;
        float oppositeZ = terrainSize.z - playerPosition.z;

        // Apply margin to the opposite position to ensure it is within the terrain boundaries
        oppositeX = Mathf.Clamp(oppositeX, margin, terrainSize.x - margin);
        oppositeZ = Mathf.Clamp(oppositeZ, margin, terrainSize.z - margin);

        // Sample the height of the terrain at the opposite position and add half the object height to the y-axis position
        float height = terrain.SampleHeight(new Vector3(oppositeX, 0, oppositeZ)) + objectHeight / 2;

        // Return the opposite position vector
        return new Vector3(oppositeX, height, oppositeZ);
    }

    float GetRandomEdgeCoordinate(float minValue, float maxValue, float distanceFromEdge)
    {
        int randomEdge = Random.Range(0, 2);
        float coordinate;

        if (randomEdge == 0)
        {
            coordinate = Random.Range(minValue, minValue + distanceFromEdge);
        }
        else
        {
            coordinate = Random.Range(maxValue - distanceFromEdge, maxValue);
        }

        return coordinate;
    }

    void OnDrawGizmos()
    {
        if (terrain != null)
        {
            // Get the terrain size and calculate the spawnable area
            Vector3 terrainSize = terrain.terrainData.size;
            Vector3 spawnableAreaStart = new Vector3(margin, 0, margin);
            Vector3 spawnableAreaSize = new Vector3(terrainSize.x - margin * 2, terrainSize.y, terrainSize.z - margin * 2);

            // Draw a wireframe cube to represent the spawnable area
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(spawnableAreaStart + spawnableAreaSize * 0.5f, spawnableAreaSize);
        }
    }
}
