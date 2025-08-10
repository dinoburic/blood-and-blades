using System.Collections.Generic;
using UnityEngine;

public class DropWeapons : MonoBehaviour
{
    public List<GameObject> Weapons;

    public void DropSwords()
    {
        for (int i = Weapons.Count - 1; i >= 0; i--)
        {
            GameObject weapon = Weapons[i];

            if (weapon == null)
            {
                Weapons.RemoveAt(i); 
                continue;
            }

            weapon.AddComponent<Rigidbody>();
            weapon.AddComponent<BoxCollider>();
            weapon.transform.parent = null;
        }
    }
}
