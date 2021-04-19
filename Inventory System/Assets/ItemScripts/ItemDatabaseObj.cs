using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Database", menuName = "Inventory System/Database")]
public class ItemDatabaseObj : ScriptableObject, ISerializationCallbackReceiver
{
    public ItemObject[] itemsObjects;


    //updates the 
    [ContextMenu("Update ID's")]
    public void UpdateID()
    {
        for (int i = 0; i < itemsObjects.Length; i++)
        {
            if (itemsObjects[i].data.id != i)
                itemsObjects[i].data.id = i;
        }
    }

    public void OnAfterDeserialize()
    {
        UpdateID();
    }

    public void OnBeforeSerialize()
    {
        
    }
}
