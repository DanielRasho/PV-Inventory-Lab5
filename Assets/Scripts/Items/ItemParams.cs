using UnityEngine;

[CreateAssetMenu(fileName = "Coffee", menuName = "Scriptable Objects/ItemParams")]
public class ItemParams : ScriptableObject
{
    [SerializeField] public AudioClip GetItemSFX;
    [SerializeField] public Sprite icon;
    [SerializeField] public string name;
    [SerializeField] public bool addToInventory;
    [SerializeField] public Item.PickupType pickupType;
}
