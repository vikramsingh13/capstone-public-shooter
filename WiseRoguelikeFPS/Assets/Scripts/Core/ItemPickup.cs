using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item;

    public bool add = true;

    //Temporary Class for Item testing
    private void OnTriggerEnter(Collider other)
    {
        if(add)
        {
            Inventory.instance.AddItem(item);
        }
        else
        {
            Inventory.instance.RemoveItem(item);
        }
        

        Destroy(gameObject);
    }
}
