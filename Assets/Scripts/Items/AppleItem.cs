using System;
using UnityEngine;

public class AppleItem : Item
{
    private EntityID entityID;
    public override string ID => entityID != null ? entityID.ID : "";

    private void Awake()
    {
        entityID = GetComponent<EntityID>();
    }

    protected override void Use()
    {
        
    }
}
