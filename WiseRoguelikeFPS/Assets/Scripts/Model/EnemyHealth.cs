using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] float hitpoints = 100f;
    [SerializeField] float maxHealth = 100f;
    private HealthBar healthBar;

    //Event to be invoked when the enemy dies
    public event Action<GameObject> OnDeath = delegate { };

    void Start()
    {
        healthBar = GetComponentInChildren<HealthBar>();
    }

    public void TakeDamage(float damage)
    {
        hitpoints -= damage;
        if (hitpoints <= 0)
        {
            //make sure hp is never negative
            hitpoints = 0;
            GetComponent<Animator>().SetTrigger("attack");
            DestroyEnemy();
        }

        //Sets the health bar value and color
        healthBar.SetHealthBar(maxHealth, hitpoints);
    }

    void DestroyEnemy()
    {
        //Invoke the OnDeath event before destroying the enemy
        OnDeath(gameObject);
        Destroy(gameObject);
    }
}