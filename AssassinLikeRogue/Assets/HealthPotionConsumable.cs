using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotionConsumable : Item
{
    public int recoverAmount;

    public void Activate()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<Health>().ModifyHealth(recoverAmount);
    }
}
