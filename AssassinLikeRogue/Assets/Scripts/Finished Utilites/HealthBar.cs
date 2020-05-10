using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private float health;
    [SerializeField] private float maxHealth;
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI hpTextMesh;
    [SerializeField] private float healthBarFillSpeed = 0.3f;


    private void OnEnable()
    {
        Image [] imageArr = GetComponentsInChildren<Image>();
        hpTextMesh = GetComponentInChildren<TextMeshProUGUI>();

        foreach (Image i in imageArr)
            if (i.name == "HpFill")
                fillImage = i;
    }

    public void SetHealth(float health)
    {
        this.health = health;
        if (hpTextMesh != null)
            hpTextMesh.SetText("HP: " + health + " / " + maxHealth);
        fillImage.fillAmount = 1;
    }

    public void SetMaxHealth(float maxHealth)
    {
        this.maxHealth = maxHealth;
        hpTextMesh.SetText("HP: " + health + " / " + maxHealth);
    }

    public void HandleHealthChanged(float pct)
    {
        StartCoroutine(ChangeToPct(pct));
    }

    public IEnumerator ChangeToPct(float pct)
    {
        float preChangePct = fillImage.fillAmount;
        float elapsed = 0f;

        while (elapsed < healthBarFillSpeed)
        {
            elapsed += Time.deltaTime;
            fillImage.fillAmount = Mathf.Lerp(preChangePct, pct, elapsed / healthBarFillSpeed);
            yield return null;
        }
        fillImage.fillAmount = pct;
    }    
}
