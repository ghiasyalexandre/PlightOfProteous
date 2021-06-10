using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Color lowColor;
    [SerializeField] private Color highColor;
    [SerializeField] private Vector3 offset;

    public void SetHealth(float health, float maxHealth)
    {
        if (GetComponent<IEnemy>().Aggro)
        {
            slider.gameObject.SetActive(true);
            text.gameObject.SetActive(true);
        }

        slider.value = health;
        slider.maxValue = maxHealth;
        text.SetText(health + "  /  " + maxHealth);
        slider.fillRect.GetComponentInChildren<Image>().color = Color.Lerp(lowColor, highColor, slider.normalizedValue);
    }
}
