using UnityEngine;

public class DamageableEntity : MonoBehaviour
{
    [SerializeField]
    protected float _currentHealth = 100f;
    [SerializeField]
    protected string _name = "DamageableEntity";

    public virtual void TakeDamage(float damageAmount)
    {
        _currentHealth -= damageAmount;
        //Debug.Log(_name + " took " + damageAmount + " damage. Health is now " + _currentHealth + ".");

        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            Die();
        }
    }

    protected void Die()
    {
        Debug.Log(_name + " was killed.");
        //temp: dont delete the player for now
        if(_name != "Player")
        {
            Destroy(gameObject);
        }
    }
}