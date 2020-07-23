using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    [HideInInspector] public GameObject player;
    public int Difficulty;
    public int AwardSeed;
    public int Level;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        InitGame();
    }

    void InitGame()
    {
        Level = 0;
    }

    public void GetRandomItem()
    {

    }
}
