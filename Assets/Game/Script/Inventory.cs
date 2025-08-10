using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;
    public static InventoryUI InstanceUI;

    public List<InventoryItem> items = new List<InventoryItem>();
    public int maxSlots = 20;
  
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public bool AddItem(Item itemToAdd)
    {
        if (itemToAdd.isStackable)
        {
            foreach (InventoryItem invItem in items)
            {
                if (invItem.item == itemToAdd && invItem.quantity < itemToAdd.maxStack)
                {
                    invItem.quantity++;
                    return true;
                    InstanceUI.RefreshUI();

                }
            }
        }

        if (items.Count >= maxSlots)
        {
            Debug.Log("Inventory full");
            return false;
        }

        items.Add(new InventoryItem(itemToAdd));
        return true;
    }

    public void RemoveItem(Item itemToRemove)
    {
        InventoryItem toRemove = items.Find(i => i.item == itemToRemove);
        if (toRemove != null)
        {
            toRemove.quantity--;
            if (toRemove.quantity <= 0)
                items.Remove(toRemove);
        }
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) // Pritisni I da pokaže torbu
        {
           
            foreach (var invItem in items)
            {
                Debug.Log(invItem.item.itemName + " x" + invItem.quantity);
            }
        }
    }

}