using UnityEngine;
using Zenject;

public class EnemyMeleeAttackHitbox : RangeIndicator
{
    private Enemy _enemy;
    private float _attackDamage;
    public virtual float AttackDamage
    {
        get { return _attackDamage; }
        set { _attackDamage = value; }
    }
    public void Init(Enemy enemy)
    {
        _enemy = enemy;
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<DamageableEntity>() != null)
        {
            if (other.gameObject.GetComponent<Player>())
            {
                _enemy.InvokeCombatEvent(other.gameObject, _attackDamage);
            }
        }
    }
}