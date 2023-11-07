using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ItemType
{
    StatModifier,
    Special
}

//Item class is abstract base class of all items, this holds all data that all items should have
public abstract class Item : ScriptableObject
{
    public int id;

    public ItemType type;

    public string itemName;

    [TextArea(20, 20)]
    public string tooltip;

    public bool stackable;

    public int quantity;

    public Sprite icon;
}
