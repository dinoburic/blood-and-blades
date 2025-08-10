using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance;
    
    public Item currentItem;

    public Transform weaponHolder; 
    private GameObject currentWeapon;
    public int currentDamage = 30;


    private void Awake()
    {
        Instance = this;
    }

    public void Equip(Item item)
    {
        
        foreach (Transform child in weaponHolder)
        {
            Destroy(child.gameObject);
        }

        
        if (item.prefab != null)
        {
            GameObject newWeapon = Instantiate(item.prefab, weaponHolder);
            newWeapon.transform.localPosition = Vector3.zero;
            currentDamage = item.damage;

            if (item.name == "Sword22") 
            {
                newWeapon.transform.localRotation = Quaternion.Euler(0, 0, -36.91f);
                newWeapon.transform.localScale = new Vector3(-1e-10f, -0.017f, -0.07f);
            } else if (item.name == "Sword3")
            {
                newWeapon.transform.localPosition = new Vector3(0, -0.004f, 0f);
                newWeapon.transform.localRotation = Quaternion.Euler(0, 0, 146.83f);
                newWeapon.transform.localScale = new Vector3(0.02f, 0.016f, -0.03f);
            }
            else
            {
                newWeapon.transform.localRotation = Quaternion.identity;
                newWeapon.transform.localScale = Vector3.one;
            }

           
            currentWeapon = newWeapon;
        }
    }

}