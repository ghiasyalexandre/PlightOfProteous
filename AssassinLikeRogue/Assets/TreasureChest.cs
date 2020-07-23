using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChestType
{
    normal,
    locked,
    red
}

public class TreasureChest : MonoBehaviour
{
    bool open;
    public ChestType type;
    public float radius;
    public Collider2D[] colliders;


    void SpawnLoot()
    {
        int safetyNet = 0;
        int amountOfLoot = 0;
        bool canSpawnHere = false;

        if (type == ChestType.normal)
            amountOfLoot = Random.Range(1, 4);
        else if (type == ChestType.locked)
            amountOfLoot = Random.Range(2, 6);

        for (int i = 0; i < amountOfLoot; i++)
        {
            while(!canSpawnHere)
            {
                canSpawnHere = PreventSpawnOverlap(transform.position); 
                if (canSpawnHere)
                    break;
            }

            safetyNet++;
            if (safetyNet > 50)
            {
                Debug.Log("Too many attempts");
                break;
            }

            var loot = GetRandomizedLoot.SharedInstance.GetRandomItem();
            loot.transform.position = transform.position;
            loot.SetActive(true);
        }
    }

    bool PreventSpawnOverlap(Vector3 spawnPos)
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            Vector3 centerPoint = colliders[i].bounds.center;
            float width = colliders[i].bounds.extents.x;
            float height = colliders[i].bounds.extents.y;

            float leftExtent = centerPoint.x - width;
            float rightExtent = centerPoint.x + width;
            float lowerExtent = centerPoint.y - height;
            float upperExtent = centerPoint.y + height;

            if (spawnPos.x >= leftExtent && spawnPos.x <= rightExtent)
            {
                if (spawnPos.y >= lowerExtent && spawnPos.y <= upperExtent)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private void Update()
    {
        colliders = Physics2D.OverlapCircleAll(transform.position, radius);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (type == ChestType.normal)
            {
                open = true;
                GetComponent<Animator>().SetBool("Open", open);
            }
            else if (type == ChestType.locked)
            {
                if (collision.GetComponent<PlayerController>().KeyCount > 0);
                {
                    collision.GetComponent<PlayerController>().KeyCount--;
                    open = true;
                    GetComponent<Animator>().SetBool("Open", open);
                }
            }
        }
    }

    void OpenChest(ChestType type)
    {
        int itemToSpawned = Random.Range(1, 4);
    }
}
