using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageHueShifter : MonoBehaviour
{
    Image image;
    private float Speed = 0.3f;
    float length = 1f;
    float saturation = 1f;
    float strength = 1.2f;
    float value = 1f;
    float opacity = 1f;

    void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        image.material.SetColor("_Color", strength * HSBColor.ToColor(new HSBColor(Mathf.PingPong(Time.time * Speed, length), saturation, value, opacity)));
    }
}
