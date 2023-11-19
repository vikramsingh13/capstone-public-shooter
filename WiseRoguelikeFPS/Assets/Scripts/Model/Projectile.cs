using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Let the code assign these instead of manually assigning it")]
    [SerializeField] private Rigidbody _projectileBody;
    [SerializeField] private Collider _projectileCollider;

    [Header("These are for testing purposes only. \n Let the code assign these.")]
    [SerializeField] private ProjectileData _projectileData;
    [SerializeField] private Vector3 _directionOfTravel;
    private float _timeToLive = 1f;
    private bool _isFiredByPlayer = true;
    //GameObject that fired the projectile -- the PlayerObject not the weapon
    private GameObject _firedByGameObject;
    //How much dmg a projectile will do is determined by WeaponData + PlayerData or EnemyData
    private float _projectileDamage = 10f;
    

    public void Init(ProjectileData projectileData,Vector3 directionOfTravel, GameObject firedBy, float projectileDamage, bool isFiredByPlayer)
    {
        _projectileData = projectileData;
        _directionOfTravel = directionOfTravel;
        _timeToLive = _projectileData.timeToLive;
        _isFiredByPlayer = isFiredByPlayer;
        _firedByGameObject = firedBy;
        _projectileDamage = projectileDamage;
    }

    void Start()
    {
        _projectileBody = GetComponent<Rigidbody>();
        MakeSureProjectileHasColliderAndIsTrigger();
    }

    // Update is called once per frame
    void Update()
    {
        if (_projectileBody == null)
        {
            _projectileBody = gameObject.AddComponent<Rigidbody>();
        }

        this.Move();

        _timeToLive -= Time.deltaTime;

        if (_timeToLive <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void Move()
    {
        //Note: no need for Time.deltaTime here since physics engine
        //already accounts for it
        _projectileBody.velocity = _directionOfTravel * _projectileData.speed;
        
    }

    //Make sure that the projectile has a collider and is a trigger
    private void MakeSureProjectileHasColliderAndIsTrigger()
    {
        //Look for a collider on the projectile
        if( _projectileCollider == null)
        {
            _projectileCollider = GetComponent<Collider>();

            //if no projectile collider are found on the projectile
            if( _projectileCollider == null ) 
            {
                //add a default sphere collider to the game object and assign that to the projectile collider variable
                gameObject.AddComponent<SphereCollider>();
                _projectileCollider = GetComponent<Collider>();
            }
        }

        //try to set the projectile collider isTrigger to true
        try
        {
            _projectileCollider.isTrigger = true;
        }
        catch( Exception ex)
        {
            Debug.LogError("Encountered the following exception, in Projectile::MakeSureProjectilesAreTriggers, trying to set the projectile collider isTrigger to true: " + ex.ToString());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Keep it simple with Damageables: enemy and player
        //delete if it hits anything else
        if ( other.GetComponent<DamageableEntity>() != null && other.GetComponent<RangeIndicator>() == null)
        {
            Debug.Log($"On Trigger enter on Projectile.cs is making past the logic check {other.gameObject.name}");
            //if the projectile is fired by the player
            if( _isFiredByPlayer )
            {
                //if the projectile hits an enemy
                if( other.GetComponent<Enemy>() != null )
                {
                    InvokeCombatEvent(other.gameObject);
                }
                //if the projectile hits the player
                else if( other.GetComponent<Player>() != null )
                {
                    //TODO: implement player self damage for rockets/grenades
                }
            }
            //if the projectile is fired by an enemy
            else
            {
                //if the projectile hits the player
                if(other.GetComponent<Player>() != null)
                {
                    InvokeCombatEvent(other.gameObject);
                }
            }
        }
        else
        {
            //TODO: check what are the different things projectiles can hit and should be destroyed e.g. navmesh obstacles, indestrutible objects like walls, etc. Then implement the logic here.
        }   
    }

    //Invokes the CombatEvent and passes in the target, source and damage
    private void InvokeCombatEvent(GameObject target)
    {
        // Create the event args
        CombatManager.CombatEventArgs args = new CombatManager.CombatEventArgs
        {
            Source = _firedByGameObject,
            Target = target,
            Damage = _projectileDamage
        };
        // Invoke the event
        CombatManager.onCombatEvent?.Invoke(this, args);
    }

    private void OnCollisionEnter(Collision collision)
    {
        /*DEFUNCT
        //If the projectile hit an enemy.
        if (collision.gameObject.tag == ("Enemy"))
        {

            //Do damage.
            EnemyHealth target = collision.transform.GetComponent<EnemyHealth>();  // Get the EnemyHealth component of the hit object
            target.TakeDamage(projectileDamage);
            //Destroy(this.gameObject);

        }

        //If it does AoE damage, deal it out.
        if (hasAreaOfEffect)
        {

            DealSplashDamage();

        }

        //Destroy the projectile at the end.
        Destroy(gameObject);
        */

    }

    private void DealSplashDamage()
    {
        /*DEFUNCT
        //Get the colliders of everything within [splashDistance] meters of the impact point.
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, splashDistance);
        
        foreach (Collider c in colliders) { 
        
            //If a collider has an EnemyHealth component.
            if(c.GetComponent<EnemyHealth>())
            {

                //Deal splash damage.
                c.GetComponent<EnemyHealth>().TakeDamage(splashDamage);

            }
        
        }
        */

    }

}
