using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{

    public float speed;
    public float bulletLife;
    public int bulletDamage;

    public bool hasAreaOfEffect;
    public float splashDamage;
    public float splashDistance;

    public Rigidbody bulletBody;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        BulletMovement();

        bulletLife -= Time.deltaTime;

        if (bulletLife < 0)
        {
            Destroy(gameObject);
        }
    }

    private void BulletMovement()
    {
        bulletBody.velocity = transform.forward * speed;
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {

            if (hasAreaOfEffect)
            {

                DealSplashDamage();

            }

            EnemyHealth target = other.transform.GetComponent<EnemyHealth>();  // Get the EnemyHealth component of the hit object
            target.TakeDamage(bulletDamage);
        }
        Destroy(this);
    }

    private void OnCollisionEnter(Collision collision)
    {

        //If the projectile hit an enemy.
        if (collision.gameObject.tag == ("Enemy"))
        {

            //Do damage.
            EnemyHealth target = collision.transform.GetComponent<EnemyHealth>();  // Get the EnemyHealth component of the hit object
            target.TakeDamage(bulletDamage);
            //Destroy(this.gameObject);

        }

        //If it does AoE damage, deal it out.
        if (hasAreaOfEffect)
        {

            DealSplashDamage();

        }

        //Destroy the projectile at the end.
        Destroy(this.gameObject);

    }

    private void DealSplashDamage()
    {

        //Get the colliders of everything within [splashDistance] meters of the impact point.
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, splashDistance);

        foreach (Collider c in colliders)
        {

            //If a collider has an EnemyHealth component.
            if (c.GetComponent<EnemyHealth>())
            {

                //Deal splash damage.
                c.GetComponent<EnemyHealth>().TakeDamage(splashDamage);

            }

        }

    }

}
