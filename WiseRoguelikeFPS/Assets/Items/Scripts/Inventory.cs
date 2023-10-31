using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    //Singleton format, add this script to an empty game object "GameManager" or something
    #region Singleton
    public static Inventory instance;

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("More than one instance of inventory!");
        }

        instance = this;
    }
    #endregion

    //Event delegates, these can be fired whenever we want
    public delegate void OnItemAdded(Item item);
    public OnItemAdded onItemAddedCallback;

    public delegate void OnItemRemoved(Item item);
    public OnItemRemoved onItemRemovedCallback;
    
    //Dictionariy of all items the player has
    public Dictionary<int, Item> items = new();

    //Add item method to add an item to the inventory, then fire the relavent event
    public void AddItem(Item item)
    {
        //If the item already exists in inventory and is stackable, increase quantity, else do nothing
        //If item does not exist, add it
        if(items.ContainsKey(item.id))
        {
            if(item.stackable)
            {
                items[item.id].quantity++;
            }
        }
        else
        {
            item.quantity++;
            items.Add(item.id, item);
        }

        //If there is a method tied to the event, fire it
        if(onItemAddedCallback != null)
        {
            Debug.Log("Item Event Invoked");
            onItemAddedCallback.Invoke(item);
        }
            
        
    }

    //If the item already exists in inventory and has more than one, decrease quantity, else remove item then fire relavent event
    //If item does not exist, add it
    public void RemoveItem(Item item)
    {
        //If item exists
        if (items.ContainsKey(item.id))
        {
            if (items[item.id].quantity > 1)
            {
                items[item.id].quantity--;
            }
            else
            {
                items.Remove(item.id);
            }

            //Fire event if there is a method tied to it
            if (onItemRemovedCallback != null)
                onItemRemovedCallback.Invoke(item);
        }
    }
}
