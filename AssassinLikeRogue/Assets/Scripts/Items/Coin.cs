using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour, Iitem
{
    public bool CanPickup()
    {
        throw new System.NotImplementedException();
    }

    public void Pickup()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().CoinCount++;
        gameObject.SetActive(false);
    }

}
