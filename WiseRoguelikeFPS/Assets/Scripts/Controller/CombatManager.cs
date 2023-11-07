using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CombatManager : Singleton<CombatManager>
{
    public class CombatEventArgs : EventArgs
    {
        public GameObject Source { get; set; }
        public GameObject Target { get; set; }
        public float Damage { get; set; }
    }

    public event EventHandler<CombatEventArgs> onTargetHit;
    //TODO: Implement the combat queue

    //Start is called before the first frame update
    void Start()
    {
        //Subscribe to the onTargetHit event
        onTargetHit += HandleTargetHitEvent;
    }

    // Update is called once per frame
    void Update()
    {
        //TODO: implement the queue and check/update end of frame for next combat event in queue
    }
    private void HandleTargetHitEvent(object sender, CombatEventArgs e)
    {
        //TODO: refactor so combat event is added to the queue
        var damageDealt = CalculateDamage(e.Source, e.Target, e.Damage);
        ApplyDamage(e.Target, damageDealt);
    }

    //Handles damage calculation based on stats, items, map mods, etc.
    private float CalculateDamage(GameObject source, GameObject target, float initialDamage)
    {
        //TODO: Implement damage calculation logic here
        return initialDamage;
    }

    // Apply the calculated damage to the target
    private void ApplyDamage(GameObject target, float damage)
    {
        //TODO: Call the ApplyDamage method of the target 
    }

    void OnDestroy()
    {
        // Unsubscribe from the onTargetHit event to prevent memory leaks
        if (onTargetHit != null)
        {
            onTargetHit -= HandleTargetHitEvent;
        }
    }
}
