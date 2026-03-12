using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinBehaviour : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float rayDistance = 1f;
    public LayerMask groundLayer;

    public Transform pointA; // левая
    public Transform pointB; // правая


    private Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    private Transform currentTarget;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //spriteRenderer = GetComponent<SpriteRenderer>();
        currentTarget = pointB;
    }

    void FixedUpdate()
    {
        if (currentTarget == null) return;

        float moveDirection = 0;

        if (transform.position.x < currentTarget.position.x)
        {
            moveDirection = 1; // движение направо
            if (spriteRenderer != null)
                spriteRenderer.flipX = false;
        }
        else if (transform.position.x > currentTarget.position.x)
        {
            moveDirection = -1; // движение налево
            if (spriteRenderer != null)
                spriteRenderer.flipX = true;
        }

        rb.velocity = new Vector2(moveDirection * moveSpeed, rb.velocity.y);

        // дошли ли до цели
        if (Mathf.Abs(transform.position.x - currentTarget.position.x) < 0.1f)
        {
            if (currentTarget == pointA)
                currentTarget = pointB;
            else
                currentTarget = pointA;
        }
    }
}
