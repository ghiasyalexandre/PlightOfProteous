using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpen : MonoBehaviour
{
    Animator doorAnimator;
    bool canOpen;
    bool isLocked;
    public bool IsLocked { set { isLocked = value; }
                          get { return isLocked; } }

    private void OnEnable()
    {
        doorAnimator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (canOpen && isLocked == false)
            doorAnimator.SetBool("Open", canOpen);
        else
            doorAnimator.SetBool("Open", canOpen);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canOpen = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canOpen = false;
        }
    }
}
