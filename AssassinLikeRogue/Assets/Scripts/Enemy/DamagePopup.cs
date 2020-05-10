 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    private const float DISAPPEAR_TIMER_MAX = 1f;
    private float disappearTimer;
    private TextMeshPro textMesh;
    private Color textColor;
    private Vector3 moveVector;
    private static int sortingOrder;

    public static DamagePopup Create(Vector3 position, int damageAmount, bool isCriticalHit)
    {
        Transform damagePopupTransform = Instantiate(Resources.Load<Transform>("Prefabs/Effects/pfDamagePopup"), position, Quaternion.identity);

        DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
        damagePopup.Setup(damageAmount, isCriticalHit);

        return damagePopup;
    }

    private void Awake()
    {
        textMesh = transform.GetComponent<TextMeshPro>();
    }

    public void Setup(int damageAmount, bool isCriticalHit)
    {
        textMesh.SetText(damageAmount.ToString());
        if (!isCriticalHit)
        {
            textMesh.fontSize = 3f;
            //textColor = new Color(245, 245, 245, 1);
            textColor = Color.white;
        }
        else
        {
            textMesh.fontSize = 5f;
            //textColor = new Color(217, 27, 27, 1);
            textColor = Color.red;
        }
        textMesh.color = textColor;
        disappearTimer = DISAPPEAR_TIMER_MAX;

        sortingOrder++;
        textMesh.sortingOrder = sortingOrder;

        moveVector = new Vector3(0.7f, 1f, 0f) * 0.2f;
    }

    private void Update()
    {
        transform.position += moveVector * Time.deltaTime;
        moveVector -= moveVector * 8f * Time.deltaTime;

        if (disappearTimer > DISAPPEAR_TIMER_MAX * 0.5f) // First half of popup animation
        {
            float increaseScaleAmount = 0.1f;
            transform.localScale += Vector3.one * increaseScaleAmount * Time.deltaTime;

        }
        else // Second half
        {
            float decreaseScaleAmount = 0.1f;
            transform.localScale -= Vector3.one * decreaseScaleAmount * Time.deltaTime;
        }

        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0)
        {
            float disappearSpeed = 3f;
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;
            if (textColor.a <= 0.1f)
                Destroy(gameObject);
        }
    }

}
