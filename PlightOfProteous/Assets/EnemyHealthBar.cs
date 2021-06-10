using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Color lowColor;
    [SerializeField] private Color highColor;
    [SerializeField] private Vector3 offset;

    public Vector3 Offset { get => offset; set => offset = value; }

    public void SetHealth(float health, float maxHealth)
    {
        slider.gameObject.SetActive(health < maxHealth);
        text.gameObject.SetActive(health < maxHealth);

        slider.value = health;
        slider.maxValue = maxHealth;
        text.SetText(health + "  /  " + maxHealth);
        slider.fillRect.GetComponentInChildren<Image>().color = Color.Lerp(lowColor, highColor, slider.normalizedValue);
    }

    private void Update()
    {
        slider.transform.position = Camera.main.WorldToScreenPoint(transform.position + offset);
        text.transform.position = slider.transform.position;
    }
}
