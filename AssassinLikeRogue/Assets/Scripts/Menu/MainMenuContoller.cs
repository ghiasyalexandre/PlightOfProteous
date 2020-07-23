using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuContoller : MonoBehaviour
{
    public string startScene;
    public GameObject mainMenu;
    public GameObject settingsMenu;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            settingsMenu.SetActive(false);
            mainMenu.SetActive(true);
        }
    }

    public void LoadNewGame()
    {
        SceneManager.LoadScene(startScene);
    }

    public void OpenOptions()
    {
        Debug.Log("Loading Options...");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
}
