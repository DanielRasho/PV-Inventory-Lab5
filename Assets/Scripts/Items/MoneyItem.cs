using System;
using UnityEngine;

public class MoneyItem : Item
{
    
    private EntityID entityId;
    public override string ID => entityId != null ? entityId.ID : "";

    [SerializeField] private int amount;
    public int Amount
    {
        get { return amount; }
    }
    
    private void Awake()
    {
        entityId = GetComponent<EntityID>();
    }
    
    protected override void Use()
    {
        
    }
}
