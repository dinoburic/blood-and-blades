using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public Item itemToPickup;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered with: " + other.name);

        if (other.CompareTag("Player"))
        {
            if (itemToPickup == null)
            {
                Debug.LogError("itemToPickup is NULL!");
                return;
            }

            if (Inventory.Instance == null)
            {
                Debug.LogError("Inventory.Instance is NULL!");
                return;
            }

            bool added = Inventory.Instance.AddItem(itemToPickup);
            if (added)
            {
                Debug.Log("Item successfully added: " + itemToPickup.itemName);
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("Item not added (inventory full?)");
            }
        }
    }

}