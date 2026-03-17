using System;
using UnityEngine;

public class MoneyItem : Item
{
    [SerializeField] private int amount;
    
    public int Amount
    {
        get { return amount; }
    }
    protected override void Use()
    {
        
    }
}
