using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class UserInterface : MonoBehaviour
{
    public InventoryObject inventory;

    //Key = GameObject. Value = InventorySlot
    public Dictionary<GameObject, InventorySlot> slotsOnInterface = new Dictionary<GameObject, InventorySlot>();

    public virtual void CreateSlots()
    {
        //overrided inside of static and dynamic inventory
    }

    private void Start()
    {
        //loops through slots in each inventory 
        for (int i = 0; i < inventory.GetSlotsFromInventory.Length; i++)
        {
            //assign each slot to correct inventory type dynamic or static
            inventory.GetSlotsFromInventory[i].parent = this;
            //subscribe to allow OnAfterUpdate to invoke this OnSlotUpdateFunction
            inventory.GetSlotsFromInventory[i].OnAfterUpdate += OnSlotUpdate;
        }
        
        //overrided inside of static and dynamic inventory
        CreateSlots();

        //create events for pointer enter and exit interface
        AddEvent(gameObject, EventTriggerType.PointerEnter, delegate { OnEnterInterface(gameObject); });
        AddEvent(gameObject, EventTriggerType.PointerExit, delegate { OnExitInterface(gameObject); });
    }

    private void OnSlotUpdate(InventorySlot _slot)
    {
        if (_slot.item.id >= 0)
        {
            _slot.slotDisplay.transform.GetChild(0).GetComponentInChildren<Image>().sprite = _slot.ItemObject.uiDisplay;
            _slot.slotDisplay.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1);
            _slot.slotDisplay.GetComponentInChildren<TextMeshProUGUI>().text = _slot.amount == 1 ? "" : _slot.amount.ToString("n0");
        }
        else
        {
            _slot.slotDisplay.transform.GetChild(0).GetComponentInChildren<Image>().sprite = null;
            _slot.slotDisplay.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0);
            _slot.slotDisplay.GetComponentInChildren<TextMeshProUGUI>().text = "";
        }
    }

    protected void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        EventTrigger.Entry eventTrigger = new EventTrigger.Entry();
        eventTrigger.eventID = type;
        eventTrigger.callback.AddListener(action);
        trigger.triggers.Add(eventTrigger);
    }


#region MyEventTriggers

    //Triggered when mouse enters a interface
    public void OnEnterInterface(GameObject obj)
    {
        MouseData.interfaceMouseIsOver = obj.GetComponent<UserInterface>();
    }

    //Triggered when mouse exits a interface
    public void OnExitInterface(GameObject obj)
    {
        MouseData.interfaceMouseIsOver = null;
    }

    //Triggered when mouse hovers over a slot
    public void OnEnter(GameObject obj)
    {
        MouseData.slotHoveredOver = obj;
    }

    //Triggered when mouse exits from hovering over a slot
    public void OnExit(GameObject obj)
    {
        MouseData.slotHoveredOver = null;
    }

    //Triggered when mouse left click is released
    public void OnMouseUp(GameObject obj)
    {
        //if no items are being dragged
        if (MouseData.tempItemBeingDragged == null)
        {
            //check if the slot we are on is a consumable
            if (slotsOnInterface[obj].ItemObject.consumable)
            {
                //if value on item is > 1 remove 1 from item
                if (slotsOnInterface[obj].amount > 1)
                    slotsOnInterface[obj].UpdateSlot(slotsOnInterface[obj].item, slotsOnInterface[obj].amount -= 1);
                //else remove item
                else
                    slotsOnInterface[obj].RemoveItem();
            }

        }
    }

    //Cretes a temp copy of a item and sets its sprite to obj passed in
    public GameObject CreateTempItem(GameObject obj)
    {
        GameObject tempItem = null;
        if (slotsOnInterface[obj].item.id >= 0)
        {
            tempItem = new GameObject();
            RectTransform rect = tempItem.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(50, 50);
            tempItem.transform.SetParent(transform.parent);
            Image img = tempItem.AddComponent<Image>();
            img.sprite = slotsOnInterface[obj].ItemObject.uiDisplay;
            img.raycastTarget = false;
        }
        return tempItem;
    }

    //Triggered when item starts being dragged
    public void OnDragStart(GameObject obj)
    {
        MouseData.tempItemBeingDragged = CreateTempItem(obj);
    }

    //Triggers while item is being dragged
    public void OnDrag(GameObject obj)
    {
        if (MouseData.tempItemBeingDragged != null)
            MouseData.tempItemBeingDragged.GetComponent<RectTransform>().position = Input.mousePosition;
    }

    //Triggered when item stops being dragged
    public void OnDragEnd(GameObject obj)
    {
        //destory the temp item we are displaying.
        Destroy(MouseData.tempItemBeingDragged);

        //If mouse is not over the top of a interface remove the item.
        if (MouseData.interfaceMouseIsOver == null)
        {
            slotsOnInterface[obj].RemoveItem();
            return;
        }

        //gets the slot we are hovered over and swaps item selected with item hovered over
        if (MouseData.slotHoveredOver)
        {
            //get the slot data of the slot we are hovered over.
            InventorySlot mouseHoverSlotData = MouseData.interfaceMouseIsOver.slotsOnInterface[MouseData.slotHoveredOver];
            inventory.SwapItems(slotsOnInterface[obj], mouseHoverSlotData);
        }
    }
#endregion
}

