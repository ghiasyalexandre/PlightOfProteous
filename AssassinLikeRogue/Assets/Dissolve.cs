using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolve : MonoBehaviour
{
    public Material defaultMaterial;
    public Material dissolve;
    SpriteRenderer[] spriteRenderer;
    float fadeSpeed = 0.2f;
    float fade = 1f;

    private void Start()
    {
        spriteRenderer = GetComponentsInChildren<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponents<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer[0] = GetComponentInParent<SpriteRenderer>();
    }

    public IEnumerator DissolveCoroutine(float _scale)
    {
        float t = 0f;
        while (t <= 1f)
        {
            t += Time.fixedDeltaTime / fadeSpeed;
            for (int i = 0; i < spriteRenderer.Length ; i++)
            {
                spriteRenderer[i].material = dissolve;
                spriteRenderer[i].material.SetFloat("_Scale", _scale);
                spriteRenderer[i].material.SetFloat("_Fade", Mathf.Lerp(1f, 0f, t));
                //Debug.Log("Dissolve Effect Activate!!! " + spriteRenderer[i].name);
            }
            yield return null;
        }
        if (t >= 1f)
        {
            for (int i = 0; i < spriteRenderer.Length; i++)
            {
                spriteRenderer[i].material = defaultMaterial;
                //Debug.Log("Dissolve Effect Activate!!! " + spriteRenderer[i].name);
            }
            yield return null;
        }
    }
}
