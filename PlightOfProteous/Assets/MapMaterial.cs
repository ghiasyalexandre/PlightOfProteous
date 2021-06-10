using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMaterial : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnEnable()
    {
        GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, 0.5f);
    }
}
