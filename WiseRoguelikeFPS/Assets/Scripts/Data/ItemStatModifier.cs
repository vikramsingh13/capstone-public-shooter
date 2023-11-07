using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stat Modifier Item Definition", menuName = "Inventory/Item_StatModifier")]
public class ItemStatModifier : Item
{
    //Subclass of Item, this type of item should be updated to potentially modify all stats of the player
    public void Awake()
    {
        type = ItemType.StatModifier;
    }

    public bool effectByPercent;

    public float hpMod;

    public float movementSpeedMod;

    public float jumpHeightMod;

    public float damageMod;
}
