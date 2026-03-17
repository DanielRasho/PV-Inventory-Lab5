using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public List<InventoryEntry> inventory = new List<InventoryEntry>();

    private Dictionary<Item.PickupType, InventoryEntry> lookup =
        new Dictionary<Item.PickupType, InventoryEntry>();

    private int money = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Item.OnItemCollected += ItemCollected;
    }

    private void ItemCollected(Item item)
    {
        if (item.data.pickupType == Item.PickupType.Money)
        {
            money += (item as MoneyItem).Amount;
            moneyUI.Instance.updateCounter(money);
            return;
        }

        if (lookup.ContainsKey(item.data.pickupType))
        {
            lookup[item.data.pickupType].count++;
        }
        else
        {
            InventoryEntry entry = new InventoryEntry
            {
                itemName = item.data.name,
                icon = item.data.icon,
                count = 1
            };

            inventory.Add(entry);
            lookup[item.data.pickupType] = entry;
        }

        HotbarUI.Instance.UpdateHotbar(inventory);
    }
}