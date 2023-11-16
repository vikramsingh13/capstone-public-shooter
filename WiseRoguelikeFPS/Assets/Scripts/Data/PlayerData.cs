using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Combat/Player", order = 1)]
public class PlayerData : CombatEntity
{
    [Header("Player weapon specification.")]
    public int maxLoadoutWeapons = 3;

    [Header("Player health and various resource specifications.")]
    public float healthRegenPerSecondFlatValue = 10f;
    public float healthRegenPerSecondPercentageIncrease = 0f;
    public float healthRegenOutOfCombatTimer = 10f;
    public float weaponHeatReductionPerSecondFlatValue = 10f;
    public float weaponHeatReductionPerSecondPercentageIncrease = 0f;
    [Header("This timer starts after weapon has reached max heat or after player has stopped firing.")]
    public float weaponHeatReductionTimer = 10f;

    [Header("Player Weapon swap specifications.")]
    public float weaponSwapCooldownFlatValue = 10f;
    public float weaponSwapCooldownPercentageDecrease = 0f;
    public float weaponHeatReductionOnSwapFlatValue = 10f;
    public float weaponHeatReductionOnSwapPercentageIncrease = 0f;
    public float weaponHeatReductionOnSwapCooldownFlatValue = 10f;
    public float weaponHeatReductionOnSwapCooldownPercentageDecrease = 0f;

    [Header("Player rocketJump specifications.")]
    public float rocketJumpFlatValue = 10f;
    public float rocketJumpPercentageIncrease = 0f;
    public float rocketJumpSpeedFlatValue = 10f;
    public float rocketJumpSpeedPercentageIncrease = 0f;
    public float rocketJumpCooldownFlatValue = 10f;
    public float rocketJumpCooldownPercentageDecrease = 0f;

    [Header("Player sprint specifications.")]
    public float sprintFlatValue = 10f;
    public float sprintPercentageIncrease = 0f;
    public float sprintSpeedFlatValue = 10f;
    public float sprintSpeedPercentageIncrease = 0f;
    public float sprintCooldownFlatValue = 10f;
    public float sprintCooldownPercentageDecrease = 0f;

    [Header("Player dash specifications.")]
    public float slideFlatValue = 10f;
    public float slidePercentageIncrease = 0f;
    public float slideSpeedFlatValue = 10f;
    public float slideSpeedPercentageIncrease = 0f;
    public float slideCooldownFlatValue = 10f;
    public float slideCooldownPercentageDecrease = 0f;

    [Header("Player armor specifications.")]
    public float armorCurrentValue = 0f;
    public float armorMaxValue = 100f;
}
