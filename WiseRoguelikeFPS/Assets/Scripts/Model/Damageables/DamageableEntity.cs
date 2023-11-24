using UnityEngine;

public class DamageableEntity : MonoBehaviour
{
    [SerializeField]
    protected float _currentHealth = 100f;
    [SerializeField]
    protected float _maxHealth = 100f;
    [SerializeField]
    protected string _name = "DamageableEntity";

    public virtual void TakeDamage(float damageAmount)
    {
        _currentHealth -= damageAmount;
        //Debug.Log(_name + " took " + damageAmount + " damage. Health is now " + _currentHealth + ".");

        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
        }

        //delegate the should enemy die logic
        ShouldEnemyDie();
    }

    protected virtual void ShouldEnemyDie()
    {
        //TODO: implement other logic that can stop enemy from dying, e.g. immortal boss phases, game state changes, certain objectives required to be met before enemy can die in the boss battle, etc.

        //for now, just destroy the enemy if hp is 0 or less
        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Debug.Log(_name + " was killed.");
        //todo: temp: dont delete the player for now
        if(_name != "Player")
        {
            Destroy(gameObject);
        }
    }
}