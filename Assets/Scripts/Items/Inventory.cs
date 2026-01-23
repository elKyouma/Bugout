using UnityEditor.Build.Reporting;
using UnityEngine;

public class Inventory
{
    private ItemType[] slots = { ItemType.None, ItemType.None };

    public (int slotID, bool success) TryAddItem(ItemType item)
    {
        for (int i = 0; i < slots.Length; i++) 
        { 
            if (slots[i] == ItemType.None)
            {
                slots[i] = item;
                return (i, true);
            }
        }
        return (-1, false);
    }

    public (int slotID, bool success) TryGetItemSlotID(ItemType item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == item)
            {
                return (i, true);
            }
        }
        return (-1, false);
    }

    public bool AreBothSlotsOccupiedByItem(ItemType item) => (slots[0] == item && slots[1] == item);
    
    public (ItemType item, bool success) TryGetItemFromSlot(int slotID)
    {
        if (slotID != 0 && slotID != 1)
            return (ItemType.None, false);

        if (slots[slotID] == ItemType.None)
            return (ItemType.None, false);

        return (slots[slotID], true);
    }

    public bool TryRemoveItemFromSlot(int slotID)
    {
        if (slotID != 0 && slotID != 1)
            return false;

        if (slots[slotID] == ItemType.None)
            return false;

        slots[slotID] = ItemType.None;
        return true;
    }
}
