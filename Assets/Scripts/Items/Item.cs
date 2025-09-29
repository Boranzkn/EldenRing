using UnityEngine;

public class Item : ScriptableObject
{
    public int itemID;
    public string itemName;
    public Sprite itemIcon;
    [TextArea] public string itemDescription;
}
