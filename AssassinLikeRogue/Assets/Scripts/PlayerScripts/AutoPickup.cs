using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoPickup : MonoBehaviour
{
    bool moveTo = false;
    Transform player;
    Iitem iitem;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    private void Start()
    {
        iitem = GetComponent<Iitem>();
    }

    private void Update()
    {
        if (moveTo)
        {
            this.transform.position = Vector2.MoveTowards(transform.position, player.position, 1.4f * Time.deltaTime);

            if (Vector2.Distance(transform.position, player.position) < 0.2f)
            {
                //if (iitem.CanPickup())
                //{
                iitem.Pickup();
                //}
                //else
                //{
                //    moveTo = false;
                //}
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Transform player = collision.transform;

        if (collision.tag == player.tag)
                moveTo = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == player.tag)
        {
            moveTo = false;
        }
    }
}
