using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldItemDatabase : MonoBehaviour
{
    public static WorldItemDatabase Instance { get; private set; }


    public WeaponItem unarmedWeapon;


    [SerializeField] private List<WeaponItem> weapons = new List<WeaponItem>();


    private List<Item> items = new List<Item>();


    private void Awake()
    {
        Instance = this;

        AddWeaponsToItemsList();
        AssignIDToItems();
    }

    private void AddWeaponsToItemsList()
    {
        foreach (var weapon in weapons)
        {
            items.Add(weapon);
        }
    }

    private void AssignIDToItems()
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].itemID = i;
        }
    }

    public WeaponItem GetWeaponByID(int ID)
    {
        return weapons.FirstOrDefault(weapon => weapon.itemID == ID);
    }
}
