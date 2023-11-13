using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] public Rigidbody _projectileBody;

    [SerializeField] private ProjectileData _projectileData;
    [SerializeField] private Vector3 _directionOfTravel;
    private float _timeToLive = 1f;

    public void Init(ProjectileData projectileData,Vector3 directionOfTravel)
    {
        _projectileData = projectileData;
        _directionOfTravel = directionOfTravel;
        _timeToLive = _projectileData.timeToLive;
    }

    void Start()
    {
        _projectileBody = GetComponent<Rigidbody>();
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

    private void OnTriggerEnter(Collider other)
    {
        /*DEFUNCT
        if(other.CompareTag("Enemy"))
        {

            if (hasAreaOfEffect)
            {

                DealSplashDamage();

            }

            EnemyHealth target = other.transform.GetComponent<EnemyHealth>();  // Get the EnemyHealth component of the hit object
            target.TakeDamage(projectileDamage);
        }
        Destroy(gameObject);
        */
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
