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
    public ChestType type;
    public float radius;
    public Collider2D[] colliders;
    public AnimatorOverrideController lockChestAnimator;
    bool open;
    bool openedAlready;
    float fade = 1f;
    float fadeTime;
    SpriteRenderer spriteRenderer;

    public int total;
    public int randomNumber;
    public int[] table = { // Loot probability table
        40,  // Small Potion
        25,  // Coin
        20,  // Key
        10,  // Gem
        5    // Large Potion
    };

    private void OnEnable()
    {
        float randNum = Random.Range(0, 1);
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (randNum <= 0.8f)
            type = ChestType.normal;
        else
        {
            type = ChestType.locked;
            GetComponent<Animator>().runtimeAnimatorController = lockChestAnimator;
        }
    }

    private void Update()
    {
        if (open)
        {
            Fade();
            if (openedAlready == false)
            {
                SpawnLoot();
                openedAlready = true;
            }
        }
    }

    ObjectToPool GetObjectFromIndex(int index)
    {
        if (index == 0)
            return ObjectToPool.HpPot;
        else if (index == 1)
            return ObjectToPool.Coin;
        else if (index == 2)
            return ObjectToPool.Key;
        else if (index == 3)
            return ObjectToPool.Gem;
        else if (index == 4)
            return ObjectToPool.LargeHpPot;
        else
            return ObjectToPool.HpPot;
    }

    void SpawnLoot()
    {
        int amountOfLoot = 0;
        Vector3 spawnPos = transform.position;

        // Chest RNG
        if (type == ChestType.normal)
            amountOfLoot = Random.Range(1, 5);
        else
            amountOfLoot = Random.Range(2, 7);

        for (int i = 0; i < amountOfLoot; i++)
        {
            Vector3 randomPos = (Vector3)Random.insideUnitCircle / 6f;
            spawnPos = transform.position + randomPos;

            GameObject loot = GetRandomItem();

            loot.SetActive(true);
            loot.transform.position = spawnPos;
            //Debug.Log(loot.name);
        }
    }

    public GameObject GetRandomItem()
    {
        foreach (var item in table)
        {
            total += item;
        }

        //Debug.Log("Total table weight: " + total);
        randomNumber = Random.Range(0, total);

        for (int i = 0; i < table.Length; i++)
        {
            if (randomNumber <= table[i])
            {
                var obj = GetObjectFromIndex(i);
                var item = ObjectPooler.SharedInstance.GetPooledObject((int)obj);
                item.SetActive(true);
                return item;
            }
            else
            {
                randomNumber -= table[i];
            }
        }
        return null;
    }

    void Fade()
    {
        Color tempcolor = spriteRenderer.material.color;
        fade = Mathf.MoveTowards(fade, 0f, Time.deltaTime);
        tempcolor.a = fade;
        spriteRenderer.color = tempcolor;
        if (tempcolor.a == 0)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (type == ChestType.normal)
            {
                GetComponent<Animator>().SetBool("Open", true);
                if (openedAlready == false)
                    open = true;
            }
        }
    }
}
