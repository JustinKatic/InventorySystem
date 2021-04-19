using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public InventoryObject inventory;
    public InventoryObject equipment;

    public PlayerAttribute[] attributes;
    public InterfaceType equipmentType;

    private void Start()
    {
        for (int i = 0; i < attributes.Length; i++)
        {
            attributes[i].SetParent(this);
        }
        for (int i = 0; i < equipment.GetSlotsFromInventory.Length; i++)
        {
            equipment.GetSlotsFromInventory[i].OnBeforeUpdate += OnBeforeSlotUpdate;
            equipment.GetSlotsFromInventory[i].OnAfterUpdate += OnAfterSlotUpdate;
        }
    }

    public void OnBeforeSlotUpdate(InventorySlot _slot)
    {
        if (_slot.ItemObject == null)
            return;

        if (_slot.parent.inventory.type == equipmentType)
        {
            //print(string.Concat("Removed ", _slot.ItemObject, " on ", _slot.parent.inventory.type, " Allowed Items: ", string.Join(",", _slot.AllowedItems)));
            for (int i = 0; i < _slot.item.attributes.Length; i++)
            {
                for (int j = 0; j < attributes.Length; j++)
                {
                    if (attributes[j].type == _slot.item.attributes[i].attributeType)
                        attributes[j].Value.RemoveModifier(_slot.item.attributes[i]);
                }
            }
        }
    }
    public void OnAfterSlotUpdate(InventorySlot _slot)
    {
        if (_slot.ItemObject == null)
            return;

        if (_slot.parent.inventory.type == equipmentType)
        {
            //print(string.Concat("Placed ", _slot.ItemObject, " on ", _slot.parent.inventory.type, " Allowed Items: ", string.Join(",", _slot.AllowedItems)));
            for (int i = 0; i < _slot.item.attributes.Length; i++)
            {
                for (int j = 0; j < attributes.Length; j++)
                {
                    if (attributes[j].type == _slot.item.attributes[i].attributeType)
                        attributes[j].Value.AddModifier(_slot.item.attributes[i]);
                }
            }
        }
    }

    //adds item to inventory 
    void OnTriggerEnter(Collider other)
    {
        var item = other.GetComponent<GroundItem>();
        if (item)
        {
            Item _item = new Item(item.item);
            if (inventory.AddItem(_item, 1))
                Destroy(other.gameObject);
        }
    }

    private void Update()
    {
        //SAVE
        if (Input.GetKeyDown(KeyCode.Space))
        {
            inventory.Save();
            equipment.Save();
        }

        //LOAD
        if (Input.GetKeyDown(KeyCode.L))
        {
            inventory.Load();
            equipment.Load();
        }
    }

    public void AttributeModified(PlayerAttribute attribute)
    {
        Debug.Log(string.Concat(attribute.type, "was updated! Value is now ", attribute.Value.ModifiedValue));
    }

    //on app quit clear inventory
    private void OnApplicationQuit()
    {
        inventory.Clear();
        equipment.Clear();
    }
}
