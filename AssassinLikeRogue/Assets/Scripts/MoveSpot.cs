using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSpot : MonoBehaviour
{
    public Animator[] animators;
    public Transform parentPos;
    public float speed = 0.4f;
    public float startWaitTime = 3.5f;
    private float waitTime;

    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    private void Start()
    {
        waitTime = startWaitTime;
        transform.position = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
    }

    private void Update()
    {
        parentPos.position = Vector2.MoveTowards(parentPos.position, transform.position, speed * Time.deltaTime);

        foreach (Animator animator in animators)
        {
            animator.SetBool("isPatrolling", true);

            if ((parentPos.position.x - transform.position.x) < 0) // Face Right
            {
                animator.GetComponent<SpriteRenderer>().flipX = true;
            }
            else                                                   // Face Left
            {
                animator.GetComponent<SpriteRenderer>().flipX = false;
            }
        }

        if (Vector2.Distance(parentPos.position, transform.position) < 0.1f)
        {
            foreach(Animator ani in animators)
            {
                ani.SetBool("isPatrolling", false);
            }

            if (waitTime <= 0)
            {
                transform.position = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
                waitTime = startWaitTime;
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }
    }
}
