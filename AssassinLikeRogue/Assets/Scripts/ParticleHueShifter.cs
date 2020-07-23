using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleHueShifter : MonoBehaviour
{
    private float Speed = 0.2f;
    private ParticleSystem settings;

    void Start()
    {
        settings = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        settings.startColor = HSBColor.ToColor(new HSBColor(Mathf.PingPong(Time.time * Speed, 1), 1, 1, 0.4f));
    }
}
