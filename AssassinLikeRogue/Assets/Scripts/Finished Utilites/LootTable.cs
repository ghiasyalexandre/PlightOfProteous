using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootTable : MonoBehaviour
{
    [SerializeField]
    //private Loot[] loot;

    private List<LootDrop> loot = new List<LootDrop>();

    public void ShowLoot()
    {
        //RollLoot();
    }

    //public Item RollLoot()
    //{
    //    int roll = Random.Range(0, 101);
    //    int weightSum = 0;
    //    int i = 0;
    //    foreach(LootDrop drop in loot)
    //    {
    //        weightSum += drop.Weight;
    //        if (roll < weightSum)
    //        {
    //            return ObjectPooler.SharedInstance.GetPooledObject(i)
    //        }
    //        i++;
    //    }
    //    return null;
    //}

}

public class LootDrop
{

}