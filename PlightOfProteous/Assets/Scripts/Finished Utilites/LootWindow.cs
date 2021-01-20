using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootWindow : MonoBehaviour
{
    private static LootWindow instance;

    public static LootWindow SharedInstance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<LootWindow>();
            }
            return instance;
        }
    }
}
