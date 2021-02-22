using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryComponent : MonoBehaviour
{
    public List<Item> items;
    public int money = 1000;

    public void BuyItem(Item item)
    {
        if(item.price <= money)
        {
            money -= item.price;
            items.Add(item);
        }
    }

    public void SellItem(Item item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            money += item.price;
        }
    }
}
