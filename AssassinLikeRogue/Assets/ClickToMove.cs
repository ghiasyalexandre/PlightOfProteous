using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickToMove : MonoBehaviour
{
    [SerializeField]
    [Range(0.1f, 5f)]
    private float moveSpeed = 0.4f;
    private Rigidbody2D rb;
    private bool isMoving = false;
    private Vector3 targetPosition;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Input.GetMouseButton(1))
            SetTargetPosition();

        if (isMoving)
            Move();
    }

    void SetTargetPosition()
    {
        targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetPosition.z = transform.position.z;
        isMoving = true;
    }

    void Move()
    {
        rb.velocity = targetPosition * moveSpeed;
        if (transform.position == targetPosition)
        {
            isMoving = false;
            rb.velocity = Vector3.zero;
        }
    }
}
