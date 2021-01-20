using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    public GameObject player;
    public GameObject portal;
    public int Difficulty;
    public int AwardSeed;
    private int level;
    public int Level { get { return level; } set { level = value; } }

    private int coinCount;
    private int keyCount;

    public int CoinCount { get => coinCount; set => coinCount = value; }
    public int KeyCount { get => keyCount; set => keyCount = value; }

    private int curDeadEnemies;
    public int DeadEnemies;

    public TextMeshProUGUI scoreCounter;
    public TextMeshProUGUI coinCounter;
    public TextMeshProUGUI keyCounter;
    public TextMeshProUGUI debugPos;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }


    private void Start()
    {
        InitGame();
    }

    private void LateUpdate()
    {
        if (curDeadEnemies != DeadEnemies)
        {
            curDeadEnemies = DeadEnemies;
            scoreCounter.SetText("Score: " + curDeadEnemies);
        }

        coinCounter.SetText("Coins: " + coinCount);
        keyCounter.SetText("Keys: " + keyCount);

        debugPos.SetText("X: " + (player.transform.position.x * 10).ToString("F2") + "\nY: " + (player.transform.position.y * 10).ToString("F2") + "\nZ: " + (player.transform.position.z * 10).ToString("F2"));
    }

    public Vector3 GetPlayerPosition()
    {
        return player.transform.position;
    }

    void InitGame()
    { 
        player = GameObject.FindWithTag("Player");
        DeadEnemies = 0;
        level = 1;
    }

    public void LoadPrevScene()
    {
        level -= 1;
        if (player != null && level < 2)
            player.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void LoadNextScene()
    {
        level += 1;
        if (player != null && level >= 2)
        {
            player.SetActive(true);
            player.transform.position = Vector3.zero;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
