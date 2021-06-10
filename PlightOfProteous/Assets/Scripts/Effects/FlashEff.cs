using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashEff : MonoBehaviour
{
    [SerializeField] SpriteRenderer[] spriteRenderer;
    List<Color> originalColors = new List<Color>();

    private void Start()
    {
        for (int i = 0; i < spriteRenderer.Length; i++)
        {
            originalColors.Add(spriteRenderer[i].color);
        }
    }

    public void Flash(float flashTime)
    {
        for (int i = 0; i < spriteRenderer.Length; i++)
        {
            StartCoroutine(FlashObject(spriteRenderer[i], originalColors[i], Color.white, flashTime, 0.01f));
        }
    }

    IEnumerator FlashObject(SpriteRenderer toFlash, Color originalColor, Color flashColor, float flashTime, float flashSpeed)
    {
        float flashingFor = 0f;
        Color newColor = flashColor;
        while (flashingFor <= flashTime)
        {
            toFlash.color = newColor;
            flashingFor += Time.deltaTime;
            yield return new WaitForSeconds(flashSpeed);
            flashingFor += flashSpeed;
            if (newColor == flashColor)
            {
                newColor = originalColor;
            }
            else
            {
                newColor = flashColor;
            }
        }
        toFlash.color = originalColor;
    }

}
