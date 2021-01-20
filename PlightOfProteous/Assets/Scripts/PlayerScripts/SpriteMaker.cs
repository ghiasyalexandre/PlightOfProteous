using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SpriteMaker : MonoBehaviour
{
    [Range(0f, 15f)]
    public float intensity = 3f;

    public SpriteRenderer[] frameRenderers;

    [ColorUsageAttribute(true, true)]
    public Color[] colorArray;
    public float[] glowArray = { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f};

    Texture2D texture;

    float startUpdateTime = 0f;
    float endUpdateTime = 5f;

    private void Start()
    {
        UpdateTextures();
    }

    private void Update()
    {
        //startUpdateTime += Time.deltaTime;

        //if (startUpdateTime > endUpdateTime)
        //{
        //    UpdateTextures();
        //    startUpdateTime = 0f;
        //}

        //frameRenderers[2].material.SetColor("_Color", HSBColor.ToColor(new HSBColor(Mathf.PingPong(Time.time * 0.3f, 1), 1, 1, 0f)));
        //frameRenderers[7].material.SetColor("_Color", HSBColor.ToColor(new HSBColor(Mathf.PingPong(Time.time * 0.3f, 1), 1, 1, 0f)));
        //frameRenderers[8].material.SetColor("_Color", HSBColor.ToColor(new HSBColor(Mathf.PingPong(Time.time * 0.3f, 1), 1, 1, 0f)));
    }


    [ContextMenu("Update Player Textures")] 
    public void UpdateTextures()
    {
        for (int i = 0; i < frameRenderers.Length; i++)
        {
            //var intensity = ((50/255) / 3f); // All White Flash
            //var intensity = ((frameRenderers[i].material.color.r + frameRenderers[i].material.color.g + frameRenderers[i].material.color.b) / 3f); // All White Flash
            //Debug.Log("Glow[" + i + "] = " + glowArray[i]);
            frameRenderers[i].material.SetColor("_Color", new Color(glowArray[i], glowArray[i], glowArray[i], 0f));
            frameRenderers[i].color = colorArray[i];
        }
    }

    public Texture2D MakeTexture(Sprite sprite, Color color)
    {
        if (sprite == null)
        {
            Debug.LogError("No Image Layer Information In Array!");
            return Texture2D.whiteTexture;
        }

        Texture2D newTexture = sprite.texture;

        Color[] colorArray = new Color[newTexture.width * newTexture.height];
        Color[] srcArray = new Color[newTexture.width * newTexture.height];
        srcArray = newTexture.GetPixels();

        for (int x = 0; x < newTexture.width; x++)
        {
            for (int y = 0; y < newTexture.height; y++)
            {
                int pixelIndex = x + (y * newTexture.width);
                Color srcPixel = srcArray[pixelIndex];

                // Apply Color if Necessary
                if (srcPixel.r != 0 && srcPixel.a != 0)
                {
                    srcPixel = ApplyColorToPixel(srcPixel, color);
                }

                // Normal Blending based on Alpha
                if (srcPixel.a == 1)
                {
                    colorArray[pixelIndex] = srcPixel;
                }
                else if (srcPixel.a > 0)
                {
                    colorArray[pixelIndex] = NormalBlend(colorArray[pixelIndex], srcPixel);
                }
           
            }
        }

        newTexture.SetPixels(colorArray);
        newTexture.Apply();

        newTexture.wrapMode = TextureWrapMode.Clamp;
        newTexture.filterMode = FilterMode.Point;

        return newTexture;
    }

    public Sprite MakeSprite(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
    }

    Color NormalBlend(Color dest, Color src)
    {
        float srcAlpha = src.a;
        float destAlpha = (1 - srcAlpha) * dest.a;
        Color destLayer = dest * destAlpha;
        Color srcLayer = src * srcAlpha;
        return destLayer + srcLayer;
    }

    Color ApplyColorToPixel(Color pixel, Color applyColor)
    {
        if (pixel.r == 1f)
            return applyColor;
        return pixel * applyColor;
    }
}
