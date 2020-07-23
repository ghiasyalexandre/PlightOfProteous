using UnityEngine;

public class HueShifter : MonoBehaviour
{
    private float Speed = 0.3f;
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        rend.material.SetColor("_Color", HSBColor.ToColor(new HSBColor(Mathf.PingPong(Time.time * Speed, 1), 1, 1, 0.2f)));
    }
}