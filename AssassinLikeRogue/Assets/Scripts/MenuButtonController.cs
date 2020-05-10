using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtonController : MonoBehaviour
{
    public int index;
    [SerializeField] bool keyDown;
    [SerializeField] int maxIndex = 3;
    [SerializeField] AudioManager audioManager;

    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    private void Update()
    {
        var menuMover = Input.GetAxis("Vertical");
        if (menuMover != 0)
        {
            if (!keyDown)
            {
                audioManager.Play("MenuScroll");
                if (menuMover < 0)
                {
                    if (index < maxIndex)
                        index++;
                    else
                        index = 0;
                }
                else if (menuMover > 0)
                {
                    if (index > 0)
                        index--;
                    else
                        index = maxIndex;
                }
                keyDown = true;
            }
        }
        else
        {
            keyDown = false;
        }
    }
}
