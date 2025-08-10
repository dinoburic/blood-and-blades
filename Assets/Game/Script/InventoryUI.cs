using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform contentPanel;

    private List<GameObject> slots = new List<GameObject>();

    private void OnEnable()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (Inventory.Instance == null)
            return;

        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        foreach (InventoryItem invItem in Inventory.Instance.items)
        {
            GameObject newSlot = Instantiate(slotPrefab, contentPanel);
            Image icon = newSlot.GetComponent<Image>();
            icon.sprite = invItem.item.icon;
            icon.preserveAspect = true;
            
            TextMeshProUGUI quantityText = newSlot.GetComponentInChildren<TextMeshProUGUI>();
            quantityText.text = invItem.quantity > 1 ? invItem.quantity.ToString() : "";

            // listener za klik
            Button button = newSlot.GetComponent<Button>();
            if (button != null)
            {
                Item itemToEquip = invItem.item;
                button.onClick.AddListener(() => EquipItem(itemToEquip));
            }
        }
    }

    private void EquipItem(Item item)
    {
        
        WeaponManager.Instance.Equip(item);
    }
}