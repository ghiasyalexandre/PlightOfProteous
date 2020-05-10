using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HotbarInterface : StaticInterface
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            UseSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            UseSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            UseSlot(2);
        if (Input.GetKeyDown(KeyCode.Alpha4))
            UseSlot(3);
        if (Input.GetKeyDown(KeyCode.Alpha5))
            UseSlot(4);
    }

    void UseSlot(int index)
    {
        slotsOnInterface = new Dictionary<GameObject, InventorySlot>();
        inventory.GetSlots[index].item.UseItem();
    }
}
