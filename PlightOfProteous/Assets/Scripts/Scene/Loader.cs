using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    public GameObject gameManager;
    public GameObject playerManager;

    private void Awake()
    {
        if (GameManager.Instance == null)
            Instantiate(gameManager);
        if (SaveManager.instance == null)
            Instantiate(playerManager);

        DontDestroyOnLoad(gameObject);
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerManager.GetComponent<SaveManager>().player = player;
        gameManager.GetComponent<GameManager>().player = player;

    }
}
