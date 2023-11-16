using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Zenject;
using static UnityEngine.Rendering.DebugUI;

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
    [SerializeField]
    private EnemyData _enemyData;
    private ProjectileManager _projectileManager;
    private bool _isAttackOnCooldown = false;
    private GameObject _meleeAttackHitbox;
    private int _nextAttackIndex = 0;
    private string _nextAttackType = "";
    private GameObject _aggroRangeIndicator;
    private bool _isAttacking = false;

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

        //find the child named "AggroRangeIndicator" and get it's sphere collider if it has it, or add a sphere collider, and set the radius to aggrorange
        _aggroRangeIndicator = transform.Find("AggroRangeIndicator")?.gameObject;
        if (_aggroRangeIndicator != null)
        {
            SphereCollider aggroSphere = _aggroRangeIndicator.GetComponent<SphereCollider>();
            if (aggroSphere != null)
            {
                aggroSphere.radius = _aggroRange;
            }
            else
            {
                _aggroRangeIndicator.AddComponent<SphereCollider>();
                _aggroRangeIndicator.GetComponent<SphereCollider>().radius = _aggroRange;
            }
        }
        else
        {
            Debug.Log($"AggroRangeIndicator not found in {gameObject.name}");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Get the NavMeshAgent component attached to this GameObject
        _navMeshAgent = GetComponent<NavMeshAgent>();
        // Get the NavMeshObstacle component attached to this GameObject
        _navMeshObstacle = GetComponent<NavMeshObstacle>();
        _navMeshObstacle.enabled = false; // Disable it initially
        GetMeleeAttackHitbox();
        PickNextAttackIndexAndType();

    }

    // Update is called once per frame
    void Update()
    {
        //if aggroTarget exists and isAggro is false,
        if(_aggroTarget != null)
        {
            _navMeshAgent.SetDestination(_aggroTarget.transform.position);
            _distanceToTarget = Vector3.Distance(transform.position, _aggroTarget.transform.position);
            if(_distanceToTarget <= SetTargetProximity())
            {
                if(!_isAttacking)
                {
                    Debug.Log("Attack !!!!");
                    _isAttacking = true;
                    AttackTarget();
                }
            }
            else
            {
                ChaseTarget();
            }
        }

        if(_aggroTarget == null && _isAggro)
        {
            _isAggro = false;
        }
    }

    //Look for child gameobject called MeleeAttackHitbox and make sure it's a trigger. Init the melee attack hitbox
    public void GetMeleeAttackHitbox()
    {
        _meleeAttackHitbox = transform.Find("MeleeAttackHitbox")?.gameObject;
        if( _meleeAttackHitbox != null )
        {
            _meleeAttackHitbox.GetComponent<Collider>().isTrigger = true;
            //Init the melee attack hitbox with the current enemy
            _meleeAttackHitbox.GetComponent<EnemyMeleeAttackHitbox>().Init(this);
            //disable the melee attack hitbox initially
            _meleeAttackHitbox.SetActive(false);
        }
        else
        {
            Debug.LogError($"Melee attack hitbox not found in {gameObject.name}");
        }
    }

    //Invokes the CombatEvent and passes in the target, source and damage
    public void InvokeCombatEvent(GameObject target, float attackDamage)
    {
        // Create the event args
        CombatManager.CombatEventArgs args = new CombatManager.CombatEventArgs
        {
            Source = this.gameObject,
            Target = target,
            Damage = attackDamage
        };
        // Invoke the event
        CombatManager.onCombatEvent?.Invoke(this, args);
    }

    // Aggro range indicator
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _aggroRange);
    }

    //Find how close to the target the enemy should be before attacking
    public float SetTargetProximity()
    {
        //Default proximity of 10f
        float proximityToTarget = 10f;
        //Don't stop right at the range. Stop closer to the target so that the enemy can attack the target
        if (_nextAttackType.ToLower() == "melee")
        {
            proximityToTarget = _enemyData.attackRange[_nextAttackIndex] - 1f;
        }
        else if (_nextAttackType.ToLower() == "ranged")
        {
            proximityToTarget = _enemyData.attackRange[_nextAttackIndex] - 5f;
        }

        return proximityToTarget;
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

    //Use melee or ranged attack depending on the attack type
    private void AttackTarget()
    {
        if (!_isAttackOnCooldown)
        {
            _isAttackOnCooldown = true;
            _isAttacking = false;
            Debug.Log($"Attacking target with {_nextAttackType} attack");
            GetComponent<Animator>().SetBool("attack", true);

            if (_nextAttackType.ToLower() == "melee")
            {
                UseMeleeAttack(_enemyData.attackRange[_nextAttackIndex], _enemyData.attackDamage[_nextAttackIndex]);
            }
            else if (_nextAttackType.ToLower() == "ranged")
            {
                //get projectile data from enemy data, and pass it to the projectile manager, direction of travel should be towards the aggro target transform
                _projectileManager.LoadAndInstantiateProjectile(_enemyData.attackProjectileDataAddress[_nextAttackIndex], _aggroTarget.transform.position - transform.position, Quaternion.identity, transform, gameObject, _enemyData.attackDamage[_nextAttackIndex], false);
            }
            //Coroutine to set the attack on cooldown if the enemy is not dead yet
            if (_enemyData != null && _enemyData.attackCooldown != null &&
    _nextAttackIndex >= 0 && _nextAttackIndex < _enemyData.attackCooldown.Count)
            {
                StartCoroutine(AttackCooldownRoutine(_enemyData.attackCooldown[_nextAttackIndex]));
            }
            else
            {
                Debug.LogError("One or more required components are null or out of range.");
            }


            //_navMeshObstacle.enabled = true; // Enable the NavMeshObstacle to avoid objects
        }
    }

    //Go through the attackType list and pick the index of the next attack
    public void PickNextAttackIndexAndType()
    {
        //TODO: don't assign a new attack until previous attack was used
        //right now we are only using the first attack in the list
        _nextAttackIndex = 0;
        if (_enemyData != null && _enemyData.attackType != null && _enemyData.attackType.Count > _nextAttackIndex)
        {
            _nextAttackType = _enemyData.attackType[_nextAttackIndex];
        }
    }

    //use melee attack. Activate the child object called melee attack hitbox, make sure it's a trigger and see if it collides with the player. If it does, invoke oncombatevent
    public void UseMeleeAttack(float meleeAttackRange, float meleeAttackDamage)
    {
        if(_meleeAttackHitbox == null)
        {
            Debug.Log($"Melee attack hitbox not found in {gameObject.name}");
        }
        else
        {
            try
            {
                //enable the hitbox
                _meleeAttackHitbox.SetActive(true);
                //set the hitbox radius to the melee attack range
                _meleeAttackHitbox.GetComponent<SphereCollider>().radius = meleeAttackRange;
                //the actual OnTriggerEnter for melee attacks are delegated to the EnemyMeleeAttackHitbox script
                _meleeAttackHitbox.gameObject.GetComponent<EnemyMeleeAttackHitbox>().AttackDamage = meleeAttackDamage;
            }
            catch
            {
                Debug.LogError($"Failed to get and use melee attack hitbox {gameObject.name}");
            }

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

    private IEnumerator AttackCooldownRoutine(float cooldownTime)
    {
        yield return new WaitForSecondsRealtime(cooldownTime);
        _isAttackOnCooldown = false;
    }

    //TODO: make a reusable coroutine class
    /*
    public IEnumerator StartCoroutine(float yieldRealSeconds)
    {
        yield return new WaitForSecondsRealtime(yieldRealSeconds);
    }
    */
}
