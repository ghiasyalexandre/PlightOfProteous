using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance = null;
    [HideInInspector] public GameObject player;
    private string keyString = "player values";
    private PlayerValues playerValues;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        //InitPlayer();
    }

    void InitPlayer()
    {
        var playerController = player.GetComponent<PlayerController>();
        player.GetComponent<Health>().SetHealth(playerValues.health);
        playerController.projectileSplit = playerValues.projectileSplit;
        playerController.attackDamage = playerValues.projectileSplit;
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
