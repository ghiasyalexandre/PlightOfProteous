using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetRandomizedLoot : MonoBehaviour
{
    //public static GetRandomizedLoot SharedInstance;

    //public int total;
    //public int randomNumber;
    //public int[] table = {
    //    40,  // Small Potion
    //    25,  // Coin
    //    20,  // Key
    //    10,  // Gem
    //    5    // Large Potion
    //};

    //void Awake()
    //{
    //    if (SharedInstance == null)
    //        SharedInstance = this;
    //    else if (SharedInstance != null)
    //        Destroy(gameObject);
    //}

    //ObjectToPool GetObjectFromIndex(int index)
    //{
    //    if (index == 0)
    //        return ObjectToPool.HpPot;
    //    else if (index == 1)
    //        return ObjectToPool.Coin;
    //    else if (index == 2)
    //        return ObjectToPool.Key;
    //    else if (index == 3)
    //        return ObjectToPool.Gem;
    //    else if (index == 4)
    //        return ObjectToPool.LargeHpPot;
    //    else
    //        return ObjectToPool.HpPot;
    //}

    //public GameObject GetRandomItem()
    //{
    //    foreach (var item in table)
    //    {
    //        total += item;
    //    }

    //    Random.Range(0, total);

    //    for (int i = 0; i < table.Length; i++)
    //    {
    //        if (randomNumber <= table[i])
    //        {
    //            var item = ObjectPooler.SharedInstance.GetPooledObject((int)GetObjectFromIndex(i));
    //            item.SetActive(true);
    //            return item;
    //        }
    //        else
    //        {
    //            randomNumber -= table[i];
    //        }
    //    }
    //    return null;
    //}
}
