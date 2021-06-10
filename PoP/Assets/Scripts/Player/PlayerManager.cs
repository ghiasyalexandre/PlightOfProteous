using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance = null;
    public GameObject player;
    private string keyString = "player values";
    private PlayerValues playerValues;
    private SpriteRenderer[] renderers;
    private int index = 4;
    SpriteMaker spriteMaker;
    public Slider red;
    public Slider green;
    public Slider blue;
    public Slider glow;

    public int Index { get => index; set => index = value; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);
        //InitPlayer();
    }

    private void Start()
    {
        spriteMaker = (SpriteMaker)FindObjectOfType(typeof(SpriteMaker));
        renderers = spriteMaker.frameRenderers;
    }

    void InitPlayer()
    {
        var playerController = player.GetComponent<PlayerController>();
        //player.GetComponent<Health>().SetHealth(playerValues.health);
        //playerController.projectileSplit = playerValues.projectileSplit;
        //playerController.attackDamage = playerValues.
    }

    public void OnEditColor()
    {
        //Debug.Log("Index: " + index);
        Color color = spriteMaker.colorArray[index];
        color.r = red.value;
        color.g = green.value;
        color.b = blue.value;
        color.a = 1f;
        spriteMaker.colorArray[index] = color;
        spriteMaker.glowArray[index] = glow.value;
        spriteMaker.UpdateTextures();
    }

    void LoadPlayerValues()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(PlayerValues));
        string text = PlayerPrefs.GetString(keyString);
        if (text.Length == 0)
        {
            playerValues = new PlayerValues();
        }
        else
        {
            using (var reader = new System.IO.StringReader(text)) {
                playerValues = serializer.Deserialize(reader) as PlayerValues;
            }
        }
            using (StringWriter sw = new StringWriter())
            {
                serializer.Serialize(sw, playerValues);
                PlayerPrefs.SetString(keyString, sw.ToString());
            }
    }

    void SavePlayerValues()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(PlayerValues));
        using (StringWriter sw = new StringWriter())
        {
            serializer.Serialize(sw, playerValues);
            PlayerPrefs.SetString(keyString, sw.ToString());
        }
    }

    void ResetPlayerValues()
    {
        PlayerPrefs.SetString(keyString, "");
    }
}
