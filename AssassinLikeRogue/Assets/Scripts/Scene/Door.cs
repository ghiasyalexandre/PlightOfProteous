using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;

public class Door : MonoBehaviour
{
    public string sceneName;
    private Animator animator;
    [SerializeField]
    private float sceneLoadDelay = 0.8f;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.name);
        if (collision.CompareTag("Player"))
        {
            if (animator != null)
                animator.SetBool("open", true);
            StartCoroutine(WaitForSceneLoad());
        }
        //animator.SetBool("open", false);
    }

    private IEnumerator WaitForSceneLoad()
    {
        yield return new WaitForSeconds(sceneLoadDelay);
        SceneManager.LoadScene(sceneName);
    }
}
