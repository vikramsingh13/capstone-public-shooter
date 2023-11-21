using UnityEngine;

public class BossEnemy : Enemy
{

    public void Update()
    {
        if(base._aggroTarget == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if(player != null)
            {
                base._aggroTarget = player.GetComponent<Player>().gameObject;
            }
        }
    }
}