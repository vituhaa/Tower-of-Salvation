using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinBehaviour : MonoBehaviour
{
    [Header("Настройки движения")]
    public float moveSpeed = 2f;
    public float rayDistance = 1f;
    public LayerMask groundLayer;

    [Header("Патрулирование")]
    public Transform pointA; // левая
    public Transform pointB; // правая

    [Header("Настройки Атаки")]
    public int collisionDamage = 1; // Урон гоблина при контакте

    private Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    private Transform currentTarget;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Автопоиск спрайта, если забыли привязать в инспекторе
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

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

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.collider.isTrigger) return;

            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(collisionDamage);
            }
            else
            {
                Debug.LogWarning("[АТАКА ГОБЛИНА] Коснулся Игрока, но скрипт PlayerHealth на нем не найден!");
            }
        }
    }
}