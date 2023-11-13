using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileData", menuName = "ScriptableObjects/ProjectileData")]
public class ProjectileData : ScriptableObject
{
    [Header("Required attributes.")]
    public float speed = 1;
    public float timeToLive = 1;
    public string projectilePrefabAddress = string.Empty;
    [Header("Optional attributes.")]
    [Tooltip("If the projectile did splash damage, what radius would it have.")]
    public float splashDamageRadius = 0f;
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