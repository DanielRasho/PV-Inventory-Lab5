using UnityEngine;

public class CoffeeItem : Item
{
    private EntityID entityId;

    public override string ID => entityId != null ? entityId.ID : "";

    private void Awake()
    {
        entityId = GetComponent<EntityID>();
    }

    protected override void Use()
    {
    }
}
