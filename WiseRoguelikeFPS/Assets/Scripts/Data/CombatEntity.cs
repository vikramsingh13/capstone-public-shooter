using UnityEngine;

[CreateAssetMenu(fileName = "NewCombatEntity", menuName = "Combat/Entity", order = 1)]
public class CombatEntity : ScriptableObject
{
    public string entityName;
    public string prefabAddressKey;

    [Header("Combat Entity Specifications")]
    public float healthFlatValue;
    public float healthPercentageIncrease;
    public float movementSpeedFlatValue;
    public float movementSpeedPercentageIncrease;

    [Header("Combat Entity audio")]
    public string idleAudioAddress;
    public string hitAudioAddress;
    public string runAudioAddress;
    public string deathAudioAddress;

    [Header("Combat Entity effects")]
    public string idleAnimation;
    public string gettingHitAnimation;
    public string runAnimation;
    public string deathAnimation;

}