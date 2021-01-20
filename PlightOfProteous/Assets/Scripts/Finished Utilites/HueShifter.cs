using UnityEngine;

public class HueShifter : MonoBehaviour
{
    private float Speed = 0.3f;
    float hue;
    float length = 1f;
    float saturation = 1;
    float strength = 1.2f;
    float value = 1;
    float opacity = 0.2f;
    Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend == null)
            rend = GetComponent<SpriteRenderer>();

        if (GetComponentInParent<EnemyAI>() != null || GetComponentInParent<BossAI>() != null)
        {
            opacity = 0f;
            strength = 1.6f;
        }
        else if (GetComponent<Portal>() != null)
        {
            opacity = 1f;
            strength = 3f;
        }
    }

    void Update()
    { 
        rend.material.SetColor("_Color", strength * HSBColor.ToColor(new HSBColor(Mathf.PingPong(Time.time * Speed, length), saturation, value, opacity)));
    }
}