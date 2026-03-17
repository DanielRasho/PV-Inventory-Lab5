using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HotbarUI : MonoBehaviour
{
    public static HotbarUI Instance;
    
    private List<HotbarSlot> slots;

    [SerializeField] public int MaxSlots;
    [SerializeField] private HotbarSlot prefabSlot;

    private void Start()
    {
        slots = new List<HotbarSlot>();
        for (int i = 0; i < MaxSlots; i++)
        {
            HotbarSlot slot = Instantiate(prefabSlot, this.transform);
            slots.Add(slot);
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateHotbar(List<InventoryEntry> items)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (i < items.Count)
                slots[i].SetItem(items[i]);
            else
                slots[i].SetItem(null);
        }
    }
}