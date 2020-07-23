using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour, Iitem
{
    public bool CanPickup()
    {
        throw new System.NotImplementedException();
    }

    public void Pickup()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().KeyCount++;
        gameObject.SetActive(false);
    }
}
