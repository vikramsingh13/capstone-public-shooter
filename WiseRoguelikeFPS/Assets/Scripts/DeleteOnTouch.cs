using UnityEngine;

public class DeleteOnTouch : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle") || other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
        }
    }
}