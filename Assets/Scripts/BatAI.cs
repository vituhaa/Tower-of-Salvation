using UnityEngine;

public class BatAI : MonoBehaviour
{
    [Header("Настройки полета")]
    [SerializeField] private float speed = 2.5f;
    public int collisionDamage = 1;

    [Header("Ссылки")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Transform playerTransform;
    private Rigidbody2D rb;
    private Animator anim;
    private bool isChasing = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        // ЛЕТИМ: Только если активирован режим погони
        if (isChasing && playerTransform != null)
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            rb.velocity = direction * speed;

            if (rb.velocity.x != 0)
            {
                spriteRenderer.flipX = rb.velocity.x > 0;
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    // --- ЗОНА ОБНАРУЖЕНИЯ (БОЛЬШОЙ КРУГ-ТРИГГЕР) ---
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerTransform = collision.transform;
            isChasing = true;

            // Включаем анимацию полета в Аниматоре
            if (anim != null) anim.SetBool("isFlying", true);
        }
    }

    // МЕТОД OnTriggerExit2D МЫ УДАЛИЛИ! Теперь мышь никогда не потеряет след.

    // --- ФИЗИЧЕСКИЙ УКУС (МАЛЕНЬКИЙ КОЛЛАЙДЕР) ---
    // --- КАСАНИЕ ИГРОКА (Постоянный контакт) ---
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.collider.isTrigger) return;

            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                // Благодаря кулдауну в PlayerHealth, урон снимется 
                // только после окончания времени неуязвимости!
                playerHealth.TakeDamage(collisionDamage);
            }
        }
    }
}