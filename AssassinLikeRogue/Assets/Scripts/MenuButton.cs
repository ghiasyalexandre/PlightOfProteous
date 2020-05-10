using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MenuButton : MonoBehaviour
{
    [SerializeField] MenuButtonController menuButtonController;
    [SerializeField] AudioManager audioManager;
    [SerializeField] Animator animator;
    [SerializeField] int thisIndex;


    private void Awake()
    {
        menuButtonController = GetComponentInParent<MenuButtonController>();
        audioManager = FindObjectOfType<AudioManager>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (menuButtonController.index == thisIndex)
        {
            animator.SetBool("selected", true);
            if (Input.GetAxis("Submit") == 1)
            {
                audioManager.Play("MenuEnter");
                animator.SetBool("pressed", true);
                if (thisIndex == 0) // New Game Button
                    SceneManager.LoadScene("Forest");
                else if (thisIndex == 1) // Continue Button
                    SceneManager.LoadScene("Forest");
                else if (thisIndex == 2) // Option Button
                    Debug.Log("Option Menu Pressed");
                else if (thisIndex == 3) // Quit Button
                    Application.Quit();
            }
            else if (animator.GetBool("pressed"))
            {
                animator.SetBool("pressed", false);
            }
        }
        else
        {
            animator.SetBool("selected", false);
        }
    }
}
