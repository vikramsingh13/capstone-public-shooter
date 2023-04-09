using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{

    public float speed;
    public float bulletLife;
    public int bulletDamage;

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
        if(other.CompareTag("Enemy"))
        {
            EnemyHealth target = other.transform.GetComponent<EnemyHealth>();  // Get the EnemyHealth component of the hit object
            target.TakeDamage(bulletDamage);
        }
        Destroy(this);
    }

}
