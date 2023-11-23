using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AI;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;
using static UnityEngine.Rendering.DebugUI;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class Enemy : DamageableEntity
{
    [SerializeField]
    protected GameObject _aggroTarget;
    [SerializeField]
    protected bool _isAggro = false; //whether the enemy is currently aggro'd by player
    [SerializeField]
    protected float _aggroRange = 30f;
    [SerializeField]
    protected NavMeshAgent _navMeshAgent;
    [SerializeField]
    protected float _distanceToTarget = Mathf.Infinity;
    [SerializeField]
    protected AudioClip attackSound;

    protected NavMeshObstacle _navMeshObstacle; // Add a NavMeshObstacle component

    protected PlayerManager _playerManager;
    [SerializeField]
    protected EnemyData _enemyData;
    protected ProjectileManager _projectileManager;
    protected bool _isAttackOnCooldown = false;
    protected GameObject _meleeAttackHitbox;
    protected int _nextAttackIndex = 0;
    protected string _nextAttackType = "";
    protected AggroRangeIndicator _aggroRangeIndicator;
    protected bool _isAttacking = false;
    protected Transform _projectileOrigin;
    protected float _attackCooldown = 2f;
    private bool _enemyDataLoaded = false;
    private AsyncOperationHandle<EnemyData> _enemyDataLoadingTask;
    private HealthBar _healthBar;
    private EnemySpawner _enemySpawner;

    [Inject]
    public void Construct(PlayerManager playerManager, ProjectileManager projectileManager)
    {
        _playerManager = playerManager;
        _projectileManager = projectileManager;
    }

    //Init will be used to initialize an ememy mob in runtime
    public void Init(EnemyData enemyData, EnemySpawner enemySpawner)
    {
        Debug.Log($"Initializing enemy {gameObject.name}");
        _enemyData = enemyData;
        //Set the aggro range of the enemy
        _aggroRange = _enemyData.aggroRange;
        _enemySpawner = enemySpawner;
    }

    // Start is called before the first frame update
    void Start()
    {

        _healthBar = GetComponentInChildren<HealthBar>();
        // Get the NavMeshAgent component attached to this GameObject
        _navMeshAgent = GetComponent<NavMeshAgent>();
        // Get the NavMeshObstacle component attached to this GameObject
        _navMeshObstacle = GetComponent<NavMeshObstacle>();
        _navMeshObstacle.enabled = false;
        //check if the enemy prefab has a child called ProjectileOrigin. If it does, use that as the projectile origin, otherwise use the enemy prefab itself as the projectile origin
        _projectileOrigin = transform.Find("ProjectileOrigin")?.gameObject.transform;
        _projectileOrigin = _projectileOrigin != null ? _projectileOrigin : transform;
        GetMeleeAttackHitbox();
        PickNextAttackIndexAndType();
        GetAggroRangeIndicator();

    }

    // Update is called once per frame
    protected void Update()
    {
        if(_enemyData == null)
        {
            Debug.Log($"Enemy data not loaded for {gameObject.name}");
            return;
        }
        if (_enemyData.isBoss)
        {
            _aggroRangeIndicator.gameObject.SetActive(false);
        }
        if (_enemyData.isBoss && _aggroTarget == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                _aggroTarget = player.GetComponent<Player>().gameObject;
            }
        }

        //if aggroTarget exists and isAggro is false,
        if (_aggroTarget != null)
        {
            _navMeshAgent.SetDestination(_aggroTarget.transform.position);
            _distanceToTarget = Vector3.Distance(transform.position, _aggroTarget.transform.position);
            if(_distanceToTarget <= SetTargetProximity())
            {
                if(!_isAttacking)
                {
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

    public void GetAggroRangeIndicator()
    {
        //find the child named "AggroRangeIndicator" and get it's sphere collider if it has it, or add a sphere collider, and set the radius to aggrorange
        _aggroRangeIndicator = transform.GetComponentInChildren<AggroRangeIndicator>();
        if (_aggroRangeIndicator != null)
        {
            _aggroRangeIndicator.Init(this, _aggroRange);
        }
        else
        {
            Debug.Log($"AggroRangeIndicator not found in {gameObject.name}");
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
            if (_nextAttackType.ToLower() == "melee")
            {
                UseMeleeAttack(_enemyData.attackRange[_nextAttackIndex], _enemyData.attackDamage[_nextAttackIndex]);
            }
            else if (_nextAttackType.ToLower() == "ranged")
            {
                //get projectile data from enemy data, and pass it to the projectile manager, direction of travel should be towards the aggro target transform
                _projectileManager.LoadAndInstantiateProjectile(_enemyData.attackProjectileDataAddress[_nextAttackIndex], _aggroTarget.transform.position - transform.position, Quaternion.identity, _projectileOrigin, gameObject, _enemyData.attackDamage[_nextAttackIndex], false);
            }
            //Coroutine to set the attack on cooldown if the enemy is not dead yet
            StartCoroutine(AttackCooldownRoutine(_attackCooldown));
            //TODO: refactor the if statement to better debug
            /*if (_enemyData != null)
            {
                if (_enemyData.attackCooldown != null)
                {
                    if (_nextAttackIndex >= 0)
                    {
                        if (_nextAttackIndex < _enemyData.attackCooldown.Count)
                        {
                            StartCoroutine(AttackCooldownRoutine(_enemyData.attackCooldown[_nextAttackIndex]));
                        }
                        else
                        {
                            Debug.Log($"Attack cooldown not found for {gameObject.name}");
                        }
                    }
                    else
                    {
                        Debug.Log($"Attack index is less than 0 for {gameObject.name}");
                    }
                }
                else
                {
                    Debug.Log($"EnemyData attack cooldown is null for {gameObject.name}");
                }
            }
            else
            {
                Debug.Log($"EnemyData is null for {gameObject.name}");
            }
            */

            //_navMeshObstacle.enabled = true; // Enable the NavMeshObstacle to avoid objects
        }

        //whether the attack went through or not, set isAttacking to false so the enemy can attack again
        _isAttacking = false;
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
    public void HandleAggroRangeIndicatorCollisions(Collider other)
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

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        //Sets the health bar value and color
        //TODO: refactor the parent logic, so the Die() method is always called after the health bar is updated
        //otherwise this try catch is necessary for when the game object is destroyed but the health bar is still trying to update
        try
        {
            _healthBar.SetHealthBar(_maxHealth, _currentHealth);
            Debug.Log($"Health bar updated for {gameObject.name} in Enemy:TakeDamage");
        }
        catch
        {
            Debug.Log($"Health bar not found on enemy. Might have been destroyed before the hp bar update. ");
        }
    }

    protected override void Die()
    {
        if(_enemySpawner == null)
        {
            Debug.Log($"Enemy spawner not found for {gameObject.name}");
        }
        else
        {
            _enemySpawner.EnemyRemoved(gameObject);
        }

        Destroy(gameObject);
    }

    //TODO: make a reusable coroutine class
    /*
    public IEnumerator StartCoroutine(float yieldRealSeconds)
    {
        yield return new WaitForSecondsRealtime(yieldRealSeconds);
    }
    */
}
