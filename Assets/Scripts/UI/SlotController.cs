using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HotbarSlot : MonoBehaviour
{
    public Image icon;
    public TMP_Text name;

    private void Awake()
    {
        icon.enabled = false;
        name.text = "";
    }

    public void SetItem(InventoryEntry entry)
    {
        if (entry == null)
        {
            icon.enabled = false;
            name.text = "";
            return;
        }

        icon.enabled = true;
        icon.sprite = entry.icon;

        string temp = entry.itemName;
        temp += entry.count > 1 ? " X" + entry.count.ToString() : "";
        name.text = temp;
    }
}
