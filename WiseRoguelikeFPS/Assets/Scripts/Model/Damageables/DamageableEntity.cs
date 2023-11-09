using UnityEngine;

public class DamageableEntity : MonoBehaviour
{
    [SerializeField]
    protected float _health = 100f;
    [SerializeField]
    protected string _name = "DamageableEntity";

    public void TakeDamage(float damageAmount)
    {
        _health -= damageAmount;

        if (_health <= 0)
        {
            _health = 0;
            Die();
        }
    }

    private void Die()
    {
        Debug.Log(_name + " was killed.");

        //destroy the GameObject or let combatManager handle it
        // Destroy(gameObject);
    }
}