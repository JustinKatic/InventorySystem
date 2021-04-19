[System.Serializable]
public class Inventory
{
    //inventory
    public InventorySlot[] Slots = new InventorySlot[24];
    public void Clear()
    {
        for (int i = 0; i < Slots.Length; i++)
        {
            Slots[i].RemoveItem();
        }
    }
}

