using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Combat/Enemy", order = 1)]
public class EnemyData: CombatEntity 
{
    [Header("Radius of the sphere player needs to be in to trigger the enemy.")]
    public float aggroRange = 30f;
    [Header("Enemy attack specifications. \n Following lists need to match number of rows.")]
    public List<string> attackType = new List<string>();
    public List<float> attackDamage = new List<float>();
    public List<float> attackCooldown = new List<float>();
    [Header("If type is ranged, then this is the range of the attack. \n If type is melee, then this is the radius of the attack.")]
    public List<float> attackRange = new List<float>();
    [Header("Only needs to be set for the rows with type = ranged.")]
    public List<string> attackProjectileDataAddress = new List<string>();
    [Header("True for those attack that can splash.")]
    public List<bool> canAttackSplash = new List<bool>();
    [Header("Only needs to be set for the rows canAttackSplash = true. \n If canAttackSplash == true, this will be by default 1.")]
    public List<float> attackMaxTarget = new List<float>();
    [Header("Only needs to be set for the rows canAttackSplash = true. \n If canAttackSplash == true, this will be by default 10.")]
    public List<float> attackSplashRadius = new List<float>();


    [Header("Enemy attack sound and animations. \n This could be left blank.")]
    public List<string> attackSound = new List<string>();
    public List<string> attackAnimation = new List<string>();

}
