using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class EnemyHealth : MonoBehaviour
{
    /*REFACTOR THIS INTO ENEMY
     * 
     * DamageableEntity that Enemy.cs inherits from does all of this
     * 
     */




    [SerializeField] private float _hitpoints = 100f;
    [SerializeField] private float _maxHealth = 100f;
    private HealthBar _healthBar;

    //Event to be invoked when the enemy dies
    public event Action<GameObject> OnDeath = delegate { };

    void Start()
    {
        _healthBar = GetComponentInChildren<HealthBar>();
    }

    public void TakeDamage(float damage)
    {
        _hitpoints -= damage;
        if (_hitpoints <= 0)
        {
            //make sure hp is never negative
            _hitpoints = 0;
            GetComponent<Animator>().SetTrigger("attack");
            DestroyEnemy();
        }

        //Sets the health bar value and color
        _healthBar.SetHealthBar(_maxHealth, _hitpoints);
    }

    void DestroyEnemy()
    {
        //Invoke the OnDeath event before destroying the enemy
        OnDeath(gameObject);
        Destroy(gameObject);
    }
}