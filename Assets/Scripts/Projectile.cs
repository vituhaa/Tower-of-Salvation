using UnityEngine;

public class Projectile : MonoBehaviour
{
    private int damage = 1;
    private Vector2 moveDirection;
    private float speed;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 direction, float launchSpeed)
    {
        moveDirection = direction;
        speed = launchSpeed;

        // Задаем скорость Rigidbody
        if (rb != null)
        {
            rb.velocity = moveDirection * speed;
        }

        // Разворачиваем спрайт шара по направлению полета (опционально)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    // Нанесение урона при контакте (коллайдер шара должен быть IS TRIGGER)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            // Уничтожаем шар после попадания в игрока
            Destroy(gameObject);
        }
    }

    // АВТОУНИЧТОЖЕНИЕ: Срабатывает, когда шар полностью улетает из поля зрения ВСЕХ камер Unity
    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}