using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePointer : MonoBehaviour
{
    public GameObject clickEffect;
    public float timeBtwSpawn = 0.1f;

    Vector3 cursorPositon;
    Animator animator;

    private void Awake()
    {
        Cursor.visible = false;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        cursorPositon = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = cursorPositon;
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, -Screen.currentResolution.width / 2, Screen.currentResolution.width / 2),
            Mathf.Clamp(transform.position.y, -Screen.currentResolution.height / 2, Screen.currentResolution.height / 2),
            -0.1f);

        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Click");
            Instantiate(clickEffect, transform.position, Quaternion.identity);
        }

        //if (timeBtwSpawn <= 0)
        //{
        //    Instantiate(trailEffect, transform.position, Quaternion.identity);
        //    timeBtwSpawn = 0.1f;
        //}
        //else
        //{
        //    timeBtwSpawn -= Time.deltaTime;
        //}
    }
}
