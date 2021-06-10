using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour, Iitem
{
    public bool CanPickup()
    {
        throw new System.NotImplementedException();
    }

    public void Pickup(GameObject player)
    {
        GameManager.Instance.KeyCount++;
        gameObject.SetActive(false);
    }
}
