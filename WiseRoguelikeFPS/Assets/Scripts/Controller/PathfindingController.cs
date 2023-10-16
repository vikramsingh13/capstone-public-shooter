using UnityEngine;
using UnityEngine.AI;

public class PathfindingController : MonoBehaviour
{
    [SerializeField]public GameObject player;
    [SerializeField]public GameObject objective;
    private NavMeshAgent playerNavMeshAgent;

    private void Start()
    {
        // Get the NavMeshAgent component attached to the player
        playerNavMeshAgent = player.GetComponent<NavMeshAgent>();

        // Set the objective as the destination for the NavMeshAgent
        SetObjective();
    }

    private void SetObjective()
    {
        if (playerNavMeshAgent != null && objective != null)
        {
            playerNavMeshAgent.SetDestination(objective.transform.position);
        }
    }
}