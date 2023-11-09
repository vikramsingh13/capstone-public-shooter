using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

public class Enemy : DamageableEntity
{
    [SerializeField]
    private GameObject _aggroTarget;
    [SerializeField]
    private bool _isAggro = false; //whether the enemy is currently aggro'd by player
    [SerializeField]
    private float _aggroRange = Mathf.Infinity;
    [SerializeField]
    private NavMeshAgent _navMeshAgent;
    [SerializeField]
    private float _distanceToTarget = Mathf.Infinity;
    [SerializeField]
    private AudioClip attackSound;

    private NavMeshObstacle _navMeshObstacle; // Add a NavMeshObstacle component

    private PlayerManager _playerManager;

    [Inject]
    public void Construct(PlayerManager playerManager)
    {
        _playerManager = playerManager;
    }

    //Init will be used to initialize an ememy mob in runtime
    public void Init(PlayerManager playerManager)
    {
        _playerManager = playerManager;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Get the NavMeshAgent component attached to this GameObject
        _navMeshAgent = GetComponent<NavMeshAgent>();
        // Get the NavMeshObstacle component attached to this GameObject
        _navMeshObstacle = GetComponent<NavMeshObstacle>();
        _navMeshObstacle.enabled = false; // Disable it initially
    }

    // Update is called once per frame
    void Update()
    {
        //if the player dies, reset aggro. 
        if (_aggroTarget == null)
        {
            _isAggro = false;
        }
        // If the enemy is provoked (i.e., the player has attacked it), engage the target
        if (_isAggro)
        {
            EngageTarget();
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
            _navMeshAgent.SetDestination(_aggroTarget.transform.position);
        }
    }

    // Set the enemy's Animator component to attack
    private void AttackTarget()
    {
        GetComponent<Animator>().SetBool("attack", true);

        if(Physics.Raycast(new Ray(transform.position + new Vector3(0, 1f, 0), Vector3.right), out RaycastHit hitInfo, 10f))
        {
            Debug.DrawLine(transform.position, hitInfo.point, Color.red, 2f);
            if (hitInfo.collider.gameObject == _aggroTarget)
            {
                Debug.Log("ATTACK PLAYER");
                hitInfo.collider.gameObject.GetComponent<DamageableEntity>().TakeDamage(10);
            }
        }
        //_navMeshObstacle.enabled = true; // Enable the NavMeshObstacle to avoid objects
    }

    //Aggro and target player when player enters the aggro range of the mob
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _aggroTarget = other.gameObject;
            _isAggro = true;
        }
    }

}
