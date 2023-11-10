using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileData", menuName = "ScriptableObjects/ProjectileData", order = 1)]
public class ProjectileData : ScriptableObject
{
    public float speed;
    public float damage;
    public float timeToLive;
    public string prefabAddress;
    public bool doesSplashDamage;
    public float splashDamageRadius;
    public bool isAffectedByGravity;
    public float gravityStrength;
    public bool isDamageDelayed;
    public float timeTillDamage;
    [Tooltip("Be careful with this; true means it can do dmg to the player, enemy, npc, etc. spawning it. Set the spawn point of projectiles carefully in editor such that it doesn't do self dmg right as it spawns.")]
    public bool doesSelfDamage;

    //can add bounce

    //some projectiles might split into smaller ones after impact

    //some projs can apply additional effects
}