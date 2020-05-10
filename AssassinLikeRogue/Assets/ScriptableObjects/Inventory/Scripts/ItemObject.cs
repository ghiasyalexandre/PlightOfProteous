using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory System/Items/New Item")]
public class ItemObject : ScriptableObject
{
    public GameObject characterDisplay;
    public Sprite uiDisplay;
    public bool isUseable;
    public bool stackable;
    public ItemType type;
    [TextArea(5, 10)] public string description;
    public Item data = new Item();

    public List<string> spriteNames = new List<string>();

    public Item CreateItem()
    {
        Item newItem = new Item(this);
        return newItem;
    }

    private void OnValidate()
    {
        spriteNames.Clear();
        if (characterDisplay == null)
            return;
        if (!characterDisplay.GetComponent<SpriteRenderer>())
            return;

        var renderer = characterDisplay.GetComponent<SpriteRenderer>();
        var bones = renderer.sprite;

        //foreach (var t in bones)
        //{
        spriteNames.Add(bones.name);
        //}
    }
}