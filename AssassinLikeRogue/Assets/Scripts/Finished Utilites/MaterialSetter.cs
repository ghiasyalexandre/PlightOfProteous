using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSetter : MonoBehaviour
{

    [ColorUsage(true, true)]
    [SerializeField] Color color;

    void OnEnable()
    {
        GetComponent<SpriteRenderer>().material.SetColor("_Color", color);
    }
}
