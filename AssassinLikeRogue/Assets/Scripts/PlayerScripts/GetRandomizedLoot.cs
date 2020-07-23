using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetRandomizedLoot : MonoBehaviour
{
    public static GetRandomizedLoot SharedInstance;

    public int total;
    public int randomNumber;
    public int[] table = {
        40,  // Half Heart
        25,  // Key
        20,  // Powerup
        10,  // Armor
        4,   // Full Heart
        1    // Map
    };

    void Awake()
    {
        SharedInstance = this;

        
    }

    ObjectToPool GetObjectFromIndex(int index)
    {
        if (index == 0)
        {
            return ObjectToPool.Arrow;
        }
        else if (index == 1)
        {
            return ObjectToPool.Arrow;
        }
        else if (index == 2)
        {
            return ObjectToPool.Arrow;
        }
        else if (index == 3)
        {
            return ObjectToPool.Arrow;
        }
        else if (index == 4)
        {
            return ObjectToPool.Arrow;
        }
        else if (index == 5)
        {
            return ObjectToPool.Arrow;
        }
        else if (index == 6)
        {
            return ObjectToPool.Arrow;
        }
        else
        {
            return ObjectToPool.Snowball;
        }
    }

    public GameObject GetRandomItem()
    {
        foreach (var item in table)
        {
            total += item;
        }

        Random.Range(0, total);

        for (int i = 0; i < table.Length; i++)
        {
            if (randomNumber <= table[i])
            {
                return ObjectPooler.SharedInstance.GetPooledObject((int)GetObjectFromIndex(i));
            }
            else
            {
                randomNumber -= table[i];
            }
        }

        return null;
    }


}
