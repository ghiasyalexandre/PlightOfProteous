using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class SpriteEditor : MonoBehaviour
{
    public static SpriteEditor instance;
    private SpriteRenderer[] renderers;
    SpriteMaker spriteMaker;
    public Slider red;
    public Slider green;
    public Slider blue;
    public Slider glow;

    private int index = 4;

    public int Index { get => index; set => index = value; }


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        spriteMaker = (SpriteMaker)FindObjectOfType(typeof(SpriteMaker));
        renderers = spriteMaker.frameRenderers;
    }

    Color GenerateSpriteColor()
    {
        return new Color(red.value, green.value, blue.value);
    }

    public void OnEditColor()
    {
        //Debug.Log("Index: " + index);
        //Color color = spriteMaker.colorArray[index];
        spriteMaker.colorArray[index] = GenerateSpriteColor();
        spriteMaker.UpdateTextures();
    }

    public void OnEditGlow()
    {
        spriteMaker.glowArray[index] = glow.value;
        spriteMaker.UpdateTextures();
    }
}
