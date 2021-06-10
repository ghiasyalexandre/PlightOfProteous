using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Collections;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.UI;

public class SaveManager : MonoBehaviour
{
    private string DATA_PATH = "/MyGame.dat";

    public static SaveManager instance = null;
    public GameObject player;
    private string keyString = "player values";
    private PlayerValues playerValues;
    private SpriteRenderer[] renderers;


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
        print("DATA PATH IS " + Application.persistentDataPath + DATA_PATH);
    }

    void InitPlayer()
    {
        var playerController = player.GetComponent<PlayerController>();
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

    void SaveData()
    {
        FileStream file = null;

        try
        {
            BinaryFormatter bf = new BinaryFormatter();

            file = File.Create(Application.persistentDataPath + DATA_PATH);

            PlayerValues p = new PlayerValues();

            bf.Serialize(file, p);
      
        } catch (Exception e)
        {
            if (e != null)
            {
                //handle exception
            }

        } finally
        {
            if (file != null)
            {
                file.Close();
            }
        }
    }

    void LoadData()
    {
        FileStream file = null;

        try
        {
            BinaryFormatter bf = new BinaryFormatter();

            file = File.Open(Application.persistentDataPath + DATA_PATH, FileMode.Open);

            playerValues = bf.Deserialize(file) as PlayerValues;
        }
        catch (Exception e)
        {
            if (e != null)
            {

            }
            
        } finally
        {
            if (file != null)
            {
                file.Close();
            }
        }
    }

    void ResetPlayerValues()
    {
        PlayerPrefs.SetString(keyString, "");
    }
}
