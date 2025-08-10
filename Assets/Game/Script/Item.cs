using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public bool isStackable = true;
    public int maxStack = 1;

    public GameObject prefab; 
    
    public int damage=30;

}