using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Prince : MonoBehaviour
{
    private float speed = 3f;
    private float jump_force = 14f;
    private bool grounded = false;
    private bool is_dead = false;

    [SerializeField] private float wallSlideSpeed = 2f;
    private bool isTouchingWall;
    private bool isWallSliding;

    [SerializeField] private Vector2 wallJumpForce = new Vector2(5f, 13f);
    private float wallJumpDirection;
    private bool isWallJumping;
    private float wallJumpTime = 0.2f;
    private float wallJumpCounter;

    [SerializeField] private Transform wallCheckPoint;
    [SerializeField] private float wallCheckRadius = 0.2f;
    [SerializeField] private LayerMask wallLayer;

    private Rigidbody2D rigid_body;
    private Animator animations;
    private SpriteRenderer sprite;
    private AudioSource audioSource;
    private float horizontalInput;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckRadius = 0.12f;
    [SerializeField] private float deathY = -10f;

    [Header("Настройки Атаки")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.6f;
    [SerializeField] private LayerMask enemyLayer;
    private float attackCooldown = 0.4f;
    private float nextAttackTime = 0f;
    private bool isAttacking = false;

    private States State
    {
        get { return (States)animations.GetInteger("state"); }
        set { animations.SetInteger("state", (int)value); }
    }

    private void Awake()
    {
        rigid_body = GetComponent<Rigidbody2D>();
        animations = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (is_dead) return;

        if (transform.position.y < deathY)
        {
            Die();
            return;
        }

        // ИСПРАВЛЕНИЕ 1: Убрали обнуление horizontalInput = 0 во время атаки.
        // Теперь ввод считывается ВСЕГДА!
        if (!isWallJumping)
        {
            horizontalInput = Input.GetAxis("Horizontal");

            if (horizontalInput != 0 && !isWallSliding)
            {
                sprite.flipX = horizontalInput < 0;
            }
        }



        if (isWallJumping)
        {
            wallJumpCounter -= Time.deltaTime;
            if (wallJumpCounter <= 0)
            {
                isWallJumping = false;
            }
        }

        // Проверка атаки на ЛКМ (ИСПРАВЛЕНИЕ 2: убрали проверку "&& grounded", чтобы атаковать можно было и в воздухе)
        if (Time.time >= nextAttackTime && Input.GetMouseButtonDown(0))
        {
            StartCoroutine(AttackRoutine());
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (grounded)
            {
                Jump();
            }
            else if (isWallSliding)
            {
                // ЖЕСТКИЙ СБРОС: Как только нажали Пробел на стене, 
                // мы СРАЗУ выключаем флаг скольжения, не дожидаясь FixedUpdate
                isWallSliding = false;

                WallJump();
            }
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        nextAttackTime = Time.time + attackCooldown;
        animations.SetInteger("state", -1);

        // ИСПРАВЛЕНИЕ 4: Удалили строчку обнуления velocity (которая тормозила Принца)
        animations.SetTrigger("AttackTrigger");

        // НАНОСИМ УРОН: Вычисляем положение точки атаки с учетом разворота
        float direction = sprite.flipX ? -1f : 1f;
        Vector2 realAttackPosition = new Vector2(
            transform.position.x + (Mathf.Abs(attackPoint.localPosition.x) * direction),
            attackPoint.position.y
        );

        // НАНОСИМ УРОН: Ищем врагов в круге
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(realAttackPosition, attackRange, enemyLayer);
        foreach (Collider2D enemyCollider in hitEnemies)
        {
            if (enemyCollider.isTrigger) continue;

            Debug.Log("НАШЛИ ВРАГА!");
            Enemy enemy = enemyCollider.GetComponentInParent<Enemy>() ?? enemyCollider.GetComponentInChildren<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(1);
            }
        }

        yield return new WaitForSeconds(attackCooldown + 0.15f);
        isAttacking = false;
    }

    private void FixedUpdate()
    {
        if (is_dead) return;

        CheckSurroundings();

        // ЖЕСТКИЙ СБРОС: Если мы на земле, мы физически не можем скользить по стене!
        if (grounded)
        {
            isWallSliding = false;
        }
        // Скользим только если: НЕ на земле И касаемся стены И жмем кнопку в сторону стены
        else if (!grounded && isTouchingWall && horizontalInput != 0)
        {
            isWallSliding = true;
            isWallJumping = false;
        }
        else
        {
            isWallSliding = false;
        }

        // --- ЛОГИКА АНИМАЦИЙ И СКОРОСТИ ---
        if (isWallSliding)
        {
            rigid_body.velocity = new Vector2(rigid_body.velocity.x, Mathf.Clamp(rigid_body.velocity.y, -wallSlideSpeed, float.MaxValue));

            if (!isAttacking) State = States.WallSlide;
        }
        else if (isWallJumping)
        {
            rigid_body.velocity = new Vector2(wallJumpDirection * wallJumpForce.x, wallJumpForce.y);
        }
        else
        {
            rigid_body.velocity = new Vector2(horizontalInput * speed, rigid_body.velocity.y);

            if (!isAttacking)
            {
                if (!grounded)
                {
                    State = States.Jump;
                }
                else if (Mathf.Abs(horizontalInput) > 0.01f)
                {
                    State = States.Run;
                }
                else
                {
                    State = States.Idle;
                }
            }
        }
    }

    private void Jump()
    {
        rigid_body.velocity = new Vector2(rigid_body.velocity.x, jump_force);
    }

    private void WallJump()
    {
        State = States.Jump;
        isWallSliding = false;
        isWallJumping = true;
        wallJumpCounter = wallJumpTime;
        wallJumpDirection = sprite.flipX ? 1f : -1f;
        rigid_body.velocity = new Vector2(wallJumpDirection * wallJumpForce.x, wallJumpForce.y);
        sprite.flipX = wallJumpDirection < 0;
    }

    private void CheckSurroundings()
    {
        // 1. Сначала четко проверяем землю
        grounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);

        // 2. Проверяем стену ТОЛЬКО если мы находимся в воздухе!
        if (wallCheckPoint != null && !grounded)
        {
            float direction = sprite.flipX ? -1f : 1f;
            Vector2 checkPosition = new Vector2(transform.position.x + (Mathf.Abs(wallCheckPoint.localPosition.x) * direction), wallCheckPoint.position.y);
            isTouchingWall = Physics2D.OverlapCircle(checkPosition, wallCheckRadius, wallLayer);
        }
        else
        {
            // Если Принц на земле, он НЕ МОЖЕТ касаться стены для скольжения
            isTouchingWall = false;
        }

        if (grounded)
        {
            isWallJumping = false;
        }
    }

    private void Die()
    {
        is_dead = true;
        State = States.Death;
        rigid_body.velocity = Vector2.zero;
        rigid_body.bodyType = RigidbodyType2D.Kinematic;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        Invoke(nameof(RestartLevel), 1.5f);
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
        }
        if (wallCheckPoint != null)
        {
            Gizmos.color = Color.blue;
            float direction = (sprite != null && sprite.flipX) ? -1f : 1f;
            Vector3 checkPosition = new Vector3(transform.position.x + (Mathf.Abs(wallCheckPoint.localPosition.x) * direction), wallCheckPoint.position.y, 0);
            Gizmos.DrawWireSphere(checkPosition, wallCheckRadius);
        }
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            float direction = (sprite != null && sprite.flipX) ? -1f : 1f;
            Vector3 checkPosition = new Vector3(transform.position.x + (Mathf.Abs(attackPoint.localPosition.x) * direction), attackPoint.position.y, 0);
            Gizmos.DrawWireSphere(checkPosition, attackRange);
        }
    }

    public void PlayHurtAnimation()
    {
        // Прерываем корутину атаки, если она шла, чтобы вернуть управление
        StopAllCoroutines();
        isAttacking = false;

        // Насильно отправляем в аниматор состояние урона
        animations.SetInteger("state", (int)States.Hurt);
    }

    public void AE_SlideDust()
    {
    }
}

public enum States
{
    Idle,
    Run,
    Jump,
    Death,
    WallSlide,
    Hurt
}