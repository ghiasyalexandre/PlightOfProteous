using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour, Iitem
{
    bool isActivated;
    GameObject player;
    SpriteRenderer rend;

    void OnEnable()
    {
        rend = GetComponentInChildren<SpriteRenderer>();
    }

    void UpgradeShots()
    {
        player.GetComponent<PlayerController>().projectileSplit += 2;
        //var shootBar = ShootBar.sharedInstance;
        //shootBar.MaxShots++;
        //shootBar.NumShots = shootBar.MaxShots;
    }

    void Update()
    {
        rend.material.SetColor("_Color", 1.6f * HSBColor.ToColor(new HSBColor(Mathf.PingPong(Time.time * 0.3f, 1), 1, 1, 0f)));
    }

    public void Pickup(GameObject player)
    {
        this.player = player;
        if (!isActivated)
        {
            StartCoroutine(DisableGem());
            UpgradeShots();
            isActivated = true;
        }
    }

    public bool CanPickup()
    {
        return true;
    }

    IEnumerator DisableGem()
    {
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }
}
