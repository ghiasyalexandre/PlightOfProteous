﻿using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheetAssigner : MonoBehaviour
{
    [SerializeField] Texture2D[] sheetsNormal;
    [SerializeField] GameObject RoomObj;
    public Vector2 roomDimensions = new Vector2(6.8f, 3.6f);
    public Vector2 gutterSize = new Vector2(0f, 0f);

    public void Assign(Room[,] rooms)
    {
        Vector3 startPos = Vector3.zero;
        
        foreach (Room room in rooms)
        {
            if (room == null)
            {
                continue;
            }
            //pick a random index for the array
            int index = Mathf.RoundToInt(Random.value * (sheetsNormal.Length - 2));
            //find position to place room
            Vector3 pos = new Vector3(room.gridPos.x * (roomDimensions.x + gutterSize.x), room.gridPos.y * (roomDimensions.y + gutterSize.y), 0);

            RoomInstance myRoom = Instantiate(RoomObj, pos, Quaternion.identity).GetComponent<RoomInstance>();
            myRoom.transform.parent = transform;
            if (room.type != 2) // Normal Room
                myRoom.Setup(sheetsNormal[index], room.gridPos, room.type, room.doorTop, room.doorBot, room.doorLeft, room.doorRight);
            else                // Boss Room
                myRoom.Setup(sheetsNormal[sheetsNormal.Length - 1], room.gridPos, room.type, room.doorTop, room.doorBot, room.doorLeft, room.doorRight);
        }
    }
}