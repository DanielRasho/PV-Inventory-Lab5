using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class Item : MonoBehaviour
{
   public virtual string ID => "";

   public enum PickupType {Coffee, Money, Contract, Apple, GreenApple }
   
   public static event Action<Item, string> OnItemCollected;
   [SerializeField] public ItemParams data;

   protected void OnTriggerEnter2D(Collider2D other)
   {
      if (other.gameObject.CompareTag("Player"))
      {
         Pick();
      }
   }
   
   public void Pick()
   {
      if (data.GetItemSFX != null)
      {
         AudioManager.Instance.PlayFX(data.GetItemSFX);
      }
      Use();
      OnItemCollected?.Invoke(this, ID);
      Destroy(gameObject);
   }
   
   // Custom logic belonging to each item
   protected abstract void Use();

}
