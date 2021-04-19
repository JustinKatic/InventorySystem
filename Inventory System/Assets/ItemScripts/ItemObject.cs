using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory System/Items/item")]
public class ItemObject : ScriptableObject
{
    public Sprite uiDisplay;
    public bool stackable;
    public bool consumable;
    public ItemTypes type;
    public Item data = new Item();


    [TextArea(15, 20)]
    public string description;

    public Item CreateItem()
    {
        Item newItem = new Item(this);
        return newItem;
    }
}

[System.Serializable]
public class Item
{
    public string name;
    public int id = -1;
    public ItemAttributes[] attributes;

    public Item()
    {
        name = "";
        id = -1;
    }
    public Item(ItemObject item)
    {
        name = item.name;
        id = item.data.id;
        attributes = new ItemAttributes[item.data.attributes.Length];

        for (int i = 0; i < attributes.Length; i++)
        {
            attributes[i] = new ItemAttributes(item.data.attributes[i].minAttributeRange, item.data.attributes[i].maxAttributeRange);
            attributes[i].attributeType = item.data.attributes[i].attributeType;
        }
    }
}
