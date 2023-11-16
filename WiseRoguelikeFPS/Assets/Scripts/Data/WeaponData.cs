using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "ScriptableObjects/WeaponData")]
public class WeaponData : ScriptableObject
{
    [Header("Unique weapon name will be used as hash map key \n by our Drop Manager. So make sure \n the weapon name here is unique.")]
    public string weaponName = string.Empty;
    public string weaponPrefabAddress = string.Empty;
    public string weaponIconAddress = string.Empty;

    [Header("Weapon Specifications")]
    public float delayBetweenPrimaryFire = 5;
    public bool isPrimaryFireHitscan = false;
    public int projectilesPerPrimaryFire = 1;
    public int damagePerPrimaryProjectile = 10;

    public bool hasSecondaryFire = false;
    public float delayBetweenSecondaryFire = 5;
    public bool isSecondaryFireHitscan = false;
    public int projectilesPerSecondaryFire = 1;
    public int damagePerSecondaryProjectile = 10;


    [Header("Ammo/Overheat Specifications")]
    public float heatPerPrimaryFire = 1f;
    public float heatPerSecondaryFire = 1f;

    [Header("Weapon specific projectile data")]
    public string primaryProjectileAddress = string.Empty;
    public string secondaryProjectileAddress = string.Empty;

    [Header("Can be left blank. Action will not happen in that case.")]
    public string primaryMuzzleFlashAddress = string.Empty;
    public string secondaryMuzzleFlashAddress = string.Empty;

    [Header("Plays at the source, i.e. the weapon barrel")]
    public string primaryFireAudioAddress = string.Empty;
    public string secondaryFireAudioAddress = string.Empty;

}
