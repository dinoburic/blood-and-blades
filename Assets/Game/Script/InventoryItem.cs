using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public Item item;
    public int quantity;

    public InventoryItem(Item newItem, int newQuantity = 1)
    {
        item = newItem;
        quantity = newQuantity;
    }
}


