using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] float hitpoints = 100f;
    [SerializeField] float maxHealth = 100f;
    private HealthBar healthBar;

    void Start()
    {
        healthBar = GetComponentInChildren<HealthBar>();
    }
    public void TakeDamage(float damage)
    {
        hitpoints -= damage;
        if (hitpoints <= 0)
        {
            GetComponent<Animator>().SetTrigger("attack");
            Invoke("DestroyEnemy", 0F);
        }
        //sets the healthbar value and color
        healthBar.SetHealthBar(maxHealth, hitpoints);

    }

    void DestroyEnemy()
    {
        Destroy(gameObject);
    }
}
