using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    public delegate void SlotUpdated(InventorySlot _slot);
    public string _slotName;
    public ItemTypes[] AllowedItems = new ItemTypes[0];
    [System.NonSerialized]
    public UserInterface parent;
    [System.NonSerialized]
    public GameObject slotDisplay;
    public Item item;
    public int amount;

    //delegates
    [System.NonSerialized]
    public SlotUpdated OnAfterUpdate;
    [System.NonSerialized]
    public SlotUpdated OnBeforeUpdate;

    public ItemObject ItemObject
    {
        get
        {
            if (item.id >= 0)
            {
                return parent.inventory.database.itemsObjects[item.id];
            }
            return null;
        }
    }
    public void UpdateSlot(Item _item, int _amount)
    {
        if (OnBeforeUpdate != null)
            OnBeforeUpdate.Invoke(this);
        item = _item;
        amount = _amount;
        if (OnAfterUpdate != null)
        {
            OnAfterUpdate.Invoke(this);
        }
    }

    public InventorySlot(Item _item, int _amount)
    {
        UpdateSlot(_item, _amount);
    }

    public void RemoveItem()
    {
        UpdateSlot(new Item(), 0);
    }

    //Add amount to value
    public void AddAmount(int value)
    {
        UpdateSlot(item, amount += value);
    }

    public bool CanPlaceInSlot(ItemObject _itemObject)
    {
        if (AllowedItems.Length <= 0 || _itemObject == null || _itemObject.data.id < 0)
            return true;
        for (int i = 0; i < AllowedItems.Length; i++)
        {
            if (_itemObject.type == AllowedItems[i])
                return true;
        }
        return false;
    }

}

