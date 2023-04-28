using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface InventoryInfo
{
    void InventoryInfoUpdate(GameItemsSlotData equipedItemSlotDataCS, GameItemsSlotData equipedItemSlotDataTE, GameItemsSlotData[] cropsSeedsSlotsData, GameItemsSlotData[] toolsEquipmentsSlotsData);
}
