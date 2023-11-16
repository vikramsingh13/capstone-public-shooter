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
    private EnemyData _enemyData;
    private ProjectileManager _projectileManager;
    private bool _isAttackOnCooldown = false;

    [Inject]
    public void Construct(PlayerManager playerManager, ProjectileManager projectileManager)
    {
        _playerManager = playerManager;
        _projectileManager = projectileManager;
    }

    //Init will be used to initialize an ememy mob in runtime
    public void Init(EnemyData enemyData)
    {
        _enemyData = enemyData;

        //Set the aggro range of the enemy
        _aggroRange = _enemyData.aggroRange;
        //get the sphere collider of the enemy and set its radius to the aggro range. If the no sphere collider is found, add one. Set the trigger to true
        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        if (sphereCollider == null)
        {
            sphereCollider = gameObject.AddComponent<SphereCollider>();
        }
        sphereCollider.radius = _aggroRange;
        sphereCollider.isTrigger = true;
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
        if (!_isAttackOnCooldown)
        {
            GetComponent<Animator>().SetBool("attack", true);

            //get projectile data from enemy data, and pass it to the projectile manager, direction of travel should be towards the aggro target transform
            _projectileManager.LoadAndInstantiateProjectile(_enemyData.attackProjectileDataAddress[0], _aggroTarget.transform.position - transform.position, Quaternion.identity, transform, gameObject, _enemyData.attackDamage[0]);

            _isAttackOnCooldown = true;
            StartCoroutine(_enemyData.attackCooldown[0]);
            _isAttackOnCooldown = false;


            //_navMeshObstacle.enabled = true; // Enable the NavMeshObstacle to avoid objects
        }
    }

    //Aggro and target player when player enters the aggro range of the mob
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Player>() != null)
        {
            _aggroTarget = other.gameObject;
            _isAggro = true;
        }
    }

    //Start coroutine
    public IEnumerator StartCoroutine(float yieldRealSeconds)
    {
        yield return new WaitForSecondsRealtime(yieldRealSeconds);
    }
}
