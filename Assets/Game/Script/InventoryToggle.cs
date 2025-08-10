using UnityEngine;

public class InventoryToggle : MonoBehaviour
{
    public GameObject inventoryUI; // Panel koji prikazuje inventar
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            bool currentlyActive = inventoryUI.activeSelf;
            inventoryUI.SetActive(!currentlyActive);

            if (!currentlyActive)
            {
                InventoryUI uiScript = inventoryUI.GetComponent<InventoryUI>();
                if (uiScript != null)
                {
                    uiScript.RefreshUI();
                }
            }
        }
    }
}