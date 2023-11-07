using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : EntityStats
{
        // Start is called before the first frame update
    new void Start()
    {
        //Base start initializes all stats
        base.Start();

        //Sets delegate methods to events, therefore the method tied to an event will run when the event is invoked
        Inventory.instance.onItemAddedCallback += ItemAdded;
        Inventory.instance.onItemRemovedCallback += ItemRemoved;
    }

    //Edit Players stats based on the items modifications
    private void ItemAdded(Item item)
    {
        //Different ways to handle different types of items
        switch(item.type)
        {
            //If the item modifies stats cast it, perform the modification either by literal or percent
            case ItemType.StatModifier:
                if(item is ItemStatModifier statModifier)
                {
                    if(statModifier.effectByPercent)
                    {
                        MovementSpeed.EditStatPercent(statModifier.movementSpeedMod / 100);
                        JumpHeight.EditStatPercent(statModifier.jumpHeightMod / 100);
                    }
                    else
                    {
                        MovementSpeed.EditStat(statModifier.movementSpeedMod);
                        JumpHeight.EditStat(statModifier.jumpHeightMod);
                    }
                }
                break;
            default:
                Debug.LogError("Invalid Item");
                break;
        }
    }

    //Edit Players stats based on the items modifications
    private void ItemRemoved(Item item)
    {
        switch (item.type)
        {
            //If the item modifies stats cast it, perform the modification either by literal or percent
            case ItemType.StatModifier:
                if (item is ItemStatModifier statModifier)
                {
                    if (statModifier.effectByPercent)
                    {
                        MovementSpeed.EditStatPercent(-1 * (statModifier.movementSpeedMod / 100));
                        JumpHeight.EditStatPercent(-1 * (statModifier.jumpHeightMod / 100));
                    }
                    else
                    {
                        MovementSpeed.EditStat(-1 * statModifier.movementSpeedMod);
                        JumpHeight.EditStat(-1 * statModifier.jumpHeightMod);
                    }
                }   
                break;
            default:
                Debug.LogError("Invalid Item");
                break;
        }
    }
}
