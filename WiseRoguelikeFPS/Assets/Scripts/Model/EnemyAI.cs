using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField]
    private Transform _target;
    [SerializeField]
    private float _aggroRange = Mathf.Infinity;
    [SerializeField]
    private NavMeshAgent _navMeshAgent;
    [SerializeField]
    private float _distanceToTarget = Mathf.Infinity;
    // Whether the enemy is aggrod by the player
    [SerializeField]
    private bool _isAggro = false;
    [SerializeField]
    private AudioClip attackSound;

    private NavMeshObstacle _navMeshObstacle; // Add a NavMeshObstacle component

    // Start is called before the first frame update
    void Start()
    {
        // Find the target object by name
        _target = GameObject.FindGameObjectWithTag("Player").transform;
        // Get the NavMeshAgent component attached to this GameObject
        _navMeshAgent = GetComponent<NavMeshAgent>();
        // Get the NavMeshObstacle component attached to this GameObject
        _navMeshObstacle = GetComponent<NavMeshObstacle>();
        _navMeshObstacle.enabled = false; // Disable it initially
    }

    // Update is called once per frame
    void Update()
    {
        if (_target == null)
        {
            _target = GameObject.FindGameObjectWithTag("Player").transform;
        }

        // Calculate the distance to the target
        _distanceToTarget = Vector3.Distance(_target.position, transform.position);

        // If the enemy is provoked (i.e., the player has attacked it), engage the target
        if (_isAggro)
        {
            EngageTarget();
        }

        if (_distanceToTarget <= _aggroRange)
        {
            _isAggro = true;
        }

    }

    // Aggro range indicator
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _aggroRange);
    }

    // Attack or chase target
    public void EngageTarget()
    {
        // Chase target outside attack range
        if (_distanceToTarget >= _navMeshAgent.stoppingDistance)
        {
            ChaseTarget();
        }

        // Attack target within attack range
        if (_distanceToTarget <= _navMeshAgent.stoppingDistance)
        {
            AttackTarget();
        }

    }

    private void ChaseTarget()
    {
        GetComponent<Animator>().SetBool("attack", false);
        GetComponent<Animator>().SetTrigger("move");

        // Check if the NavMeshAgent is enabled before setting the destination
        if (_navMeshAgent.enabled)
        {
            _navMeshObstacle.enabled = false; // Disable the NavMeshObstacle
            _navMeshAgent.SetDestination(_target.position);
        }
    }

    // Set the enemy's Animator component to attack
    private void AttackTarget()
    {
        GetComponent<Animator>().SetBool("attack", true);

        if(Physics.Raycast(new Ray(transform.position + new Vector3(0, 1f, 0), Vector3.right), out RaycastHit hitInfo, 10f))
        {
            Debug.DrawLine(transform.position, hitInfo.point, Color.red, 2f);
            if (hitInfo.collider.gameObject.CompareTag("Player"))
            {
                Debug.Log("ATTACK PLAYER");
                hitInfo.collider.gameObject.GetComponent<Player>().TakeDamage(10);
            }
        }
        //_navMeshObstacle.enabled = true; // Enable the NavMeshObstacle to avoid objects
    }

}
