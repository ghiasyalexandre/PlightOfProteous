using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoPickup : MonoBehaviour
{
    bool moveTo;
    GameObject player;
    Iitem iitem;

    public bool MoveTo { set { moveTo = value; } get { return moveTo; } }

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    private void Start()
    {
        iitem = GetComponent<Iitem>();
    }

    private void Update()
    {
        if (moveTo)
        {
            this.transform.position = Vector2.MoveTowards(transform.position, player.transform.position, 1f * Time.deltaTime);

            if (Vector2.Distance(transform.position, player.transform.position) < 0.2f)
            {
                iitem.Pickup(player);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == player.tag)
        {
            moveTo = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == player.tag)
        {
            moveTo = false;
        }
    }
}
