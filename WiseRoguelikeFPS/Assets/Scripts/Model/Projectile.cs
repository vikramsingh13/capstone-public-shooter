using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    public float speed;
    public float projectileLife;
    public int projectileDamage;

    public bool hasAreaOfEffect;
    public float splashDamage;
    public float splashDistance;


    public Rigidbody projectileBody;

    public void Init()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Move();

        projectileLife -= Time.deltaTime;

        if (projectileLife <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void Move()
    {
        projectileBody.velocity = transform.forward * speed;
        
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
