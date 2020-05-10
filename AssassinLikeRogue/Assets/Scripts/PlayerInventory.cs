using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

public class PlayerInventory : MonoBehaviour
{
    public PlayerController player;
    public GameObject inventoryCanvas;
    public InventoryObject inventory;
    public InventoryObject equipment;
    public SpriteRenderer crossHair;

    private bool isInventoryOpen = false;

    public Attribute[] attributes;

    private void Awake()
    {
        inventoryCanvas.SetActive(false);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        var groundItem = collision.GetComponent<GroundItem>();
        if (groundItem)
        {
            if (inventory.AddItem(new Item(groundItem.item), 1))
                Destroy(collision.gameObject);
        }
    }

    public void AttributeModified(Attribute attribute)
    {
        //Debug.Log(string.Concat(attribute.type, " was updated! Value is now ", attribute.value.ModifiedValue));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (isInventoryOpen) // On Close
            {
                crossHair.color = new Color(1f, 1f, 1f, 1f);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                isInventoryOpen = false;
            }
            else // On Open
            {
                crossHair.color = new Color (1f, 1f, 1f, 0f);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                isInventoryOpen = true;
            }

            inventoryCanvas.SetActive(isInventoryOpen);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            inventory.Save();
            equipment.Save();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            inventory.Load();
            equipment.Load();
        }
        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            inventory.Clear();
        }
    }

    public void OnApplicationQuit()
    {
        inventory.Clear();
        equipment.Clear();
    }
}
