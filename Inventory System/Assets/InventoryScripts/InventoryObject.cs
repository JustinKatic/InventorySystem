using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;


[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject
{
    public InterfaceType type;
    //file name to save path under.
    public string savePath;
    //list of all items
    public ItemDatabaseObj database = null;

    //returns array of slots on inventory.
    public InventorySlot[] GetSlotsFromInventory { get { return inventory.Slots; } }

    //refrence to a inventory
    public Inventory inventory;


    public bool AddItem(Item _item, int _amount)
    {
        //find items of same type in inventory
        InventorySlot slot = FindItemOnInventory(_item);

        //if item not stackable || we didnt find any items of the same type.
        if (!database.itemsObjects[_item.id].stackable || slot == null)
        {
            if (GetNumberOfEmptySlotsCount <= 0)
                return false;

            FindAndSetItemToEmptySlot(_item, _amount);
            return true;
        }
        //if we did find a item of same type add amount to it.
        slot.AddAmount(_amount);
        return true;
    }

    //loops through inventory checking if any items share the same id. returns the slot if one was found
    public InventorySlot FindItemOnInventory(Item _item)
    {
        for (int i = 0; i < GetSlotsFromInventory.Length; i++)
        {
            if (GetSlotsFromInventory[i].item.id == _item.id)
            {
                return GetSlotsFromInventory[i];
            }
        }
        return null;
    }

    //Loops through inventory counting all empty slots
    public int GetNumberOfEmptySlotsCount
    {
        get
        {
            int counter = 0;
            for (int i = 0; i < GetSlotsFromInventory.Length; i++)
            {
                if (GetSlotsFromInventory[i].item.id <= -1)
                    counter++;
            }
            return counter;
        }
    }

    //Loops through inventory slots and if finds a slot updates empty slot with item and amount passed in.
    public InventorySlot FindAndSetItemToEmptySlot(Item _item, int _amount)
    {
        for (int i = 0; i < GetSlotsFromInventory.Length; i++)
        {
            if (GetSlotsFromInventory[i].item.id <= -1)
            {
                GetSlotsFromInventory[i].UpdateSlot(_item, _amount);
                return GetSlotsFromInventory[i];
            }
        }
        return null;
    }


    //Swaps 2 items passed in
    public void SwapItems(InventorySlot item1, InventorySlot item2)
    {
        if (item2.CanPlaceInSlot(item1.ItemObject) && item1.CanPlaceInSlot(item2.ItemObject))
        {
            InventorySlot temp = new InventorySlot(item2.item, item2.amount);
            item2.UpdateSlot(item1.item, item1.amount);
            item1.UpdateSlot(temp.item, temp.amount);
        }
    }

    //Removes a item from the inventory
    public void RemoveItem(Item _item)
    {
        for (int i = 0; i < GetSlotsFromInventory.Length; i++)
        {
            if (GetSlotsFromInventory[i].item == _item)
            {
                GetSlotsFromInventory[i].UpdateSlot(null, 0);
            }
        }
    }


    //Saves file to persistant data path under name given in savePath var
    [ContextMenu("Save")]
    public void Save()
    {
        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Create, FileAccess.Write);
        formatter.Serialize(stream, inventory);
        stream.Close();
    }

    //Loads file from persistant data path from name given in savePath var
    [ContextMenu("Load")]
    public void Load()
    {
        if (File.Exists(string.Concat(Application.persistentDataPath, savePath)))
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Open, FileAccess.Read);
            Inventory newContainer = (Inventory)formatter.Deserialize(stream);
            for (int i = 0; i < GetSlotsFromInventory.Length; i++)
            {
                GetSlotsFromInventory[i].UpdateSlot(newContainer.Slots[i].item, newContainer.Slots[i].amount);
            }
            stream.Close();
        }
    }

    //Context menu function that allows us to easily clear out inventory in editor.
    [ContextMenu("Clear")]
    public void Clear()
    {
        inventory.Clear();
    }
}