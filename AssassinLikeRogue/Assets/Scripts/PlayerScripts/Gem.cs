using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour, Iitem
{
    GameObject player;
    SpriteRenderer rend;

    void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rend = GetComponentInChildren<SpriteRenderer>();
    }

    void UpgradeShots()
    {
        player.GetComponent<PlayerController>().projectileSplit += 2;
        var shootBar = ShootBar.sharedInstance;
        shootBar.MaxShots++;
        shootBar.NumShots = shootBar.MaxShots;
    }

    void Update()
    {
        rend.material.SetColor("_Color", HSBColor.ToColor(new HSBColor(Mathf.PingPong(Time.time * 0.3f, 1), 1, 1, 0f)));
    }

    public void Pickup()
    {
        if (CanPickup())
        {
            UpgradeShots();
            Destroy(gameObject);
        }
    }

    public bool CanPickup()
    {
        return true;
    }
}
