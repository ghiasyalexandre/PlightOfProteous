using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] float sceneLoadDelay = 1.2f;
    [SerializeField] float speed = 3f;
    float startTime;
    bool open;

    private void Update()
    {
        if (open)
        {
            startTime = 0f;
            if (startTime >= sceneLoadDelay)
            {
                open = false;
                GameManager.Instance.LoadNextScene();
            }
            else
            {
                startTime += Time.deltaTime;
            }
            transform.localScale += transform.localScale * Time.deltaTime * speed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            open = true;
        }
    }
}
