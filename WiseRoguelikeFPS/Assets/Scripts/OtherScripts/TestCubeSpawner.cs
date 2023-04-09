using UnityEngine;

public class TestCubeSpawner : MonoBehaviour
{
    private void Start()
    {
        SpawnTestCube();
    }

    private void SpawnTestCube()
    {
        Debug.Log("Spawning test cube");
        GameObject testCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        testCube.transform.position = new Vector3(0, 5, 0);
        testCube.transform.SetParent(transform);
    }
}
