using UnityEngine;
using Zenject;

public class AggroRangeIndicator : RangeIndicator
{
    private Enemy _enemy;
    private float _aggroRange;
    private SphereCollider _aggroSphere;

    public void Init(Enemy enemy, float aggroRange)
    {
        _enemy = enemy;
        _aggroRange = aggroRange;
        SetAggroSphere();
    }

    public void Start()
    {
        
    }

    private void SetAggroSphere()
    {
        //find the sphere collider on the game object
        //if no sphere collider is found, add one
        _aggroSphere = gameObject.GetComponent<SphereCollider>();
        if (_aggroSphere != null)
        {
            //set the radius of the sphere collider to the aggro range
            _aggroSphere.radius = _aggroRange;
            Debug.Log($"AggroRangeIndicator::Start() - SphereCollider found on {gameObject.name}. Setting radius to {_aggroRange}");
        }
        else
        {
            //add a sphere collider to the game object and set the radius to the aggro range
            _aggroSphere = gameObject.AddComponent<SphereCollider>();
            _aggroSphere.radius = _aggroRange;
            Debug.Log($"AggroRangeIndicator::Start() - No SphereCollider found on {gameObject.name}. Adding a SphereCollider component with radius {_aggroRange}");
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log($"AggroRangeIndicator::OnTriggerEnter() - {gameObject.name} collided with {other.gameObject.name}");
        Debug.Log($"enemy in onTriggerEnter in aggro range indicator: {_enemy}");

        if (_enemy != null) 
        {
            //delegate to the enemy class
            _enemy.HandleAggroRangeIndicatorCollisions(other);
        }
    }

}