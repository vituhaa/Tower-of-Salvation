using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Prince : MonoBehaviour
{
    private float speed = 3f; // скорость движения
    private float jump_force = 14f; // сила прыжка
    private bool grounded = false;
    private bool is_dead = false;

    private Rigidbody2D rigid_body; // ссылка на компонент
    private Animator animations; // ссылка на компонент анимации
    private SpriteRenderer sprite; // ссылка на компонент где изображение принца
    private AudioSource audioSource; // ссылка на компонент AudioSource
    private float horizontalInput;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckRadius = 0.12f;
    [SerializeField] private float deathY = -10f;



    private States State
    {
        get { return (States)animations.GetInteger("state"); } // получение значений из аниматора
        set { animations.SetInteger("state", (int)value); } // изменение значений
    }

    private void Awake()
    {
        rigid_body = GetComponent<Rigidbody2D>(); // получение компонента
        animations = GetComponent<Animator>(); // получение компонента
        sprite = GetComponentInChildren<SpriteRenderer>(); // получение компонента из дочернего объекта
        audioSource = GetComponent<AudioSource>(); // получение компонента AudioSource
    }

    void Start()
    {

    }

    void Update()
    {
        if (is_dead)
        {
            return;
        }

        if (transform.position.y < deathY)
        {
            Die();
            return;
        }

        horizontalInput = Input.GetAxis("Horizontal");
        if (horizontalInput != 0)
        {
            sprite.flipX = horizontalInput < 0;
        }

        if (grounded && Input.GetButtonDown("Jump"))
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {
        if (is_dead) return;
        CheckGrounded();
        float gorizontal_move = Input.GetAxis("Horizontal");
        rigid_body.velocity = new Vector2(gorizontal_move * speed, rigid_body.velocity.y);
        //if (gorizontal_move != 0)
        //{
        //    sprite.flipX = gorizontal_move < 0.0f;
        //}
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


    //private void Run()
    //{
    //    if (grounded)
    //    {
    //        State = States.Run; // состояние бега
    //    }
    //    Vector3 direction = transform.right * Input.GetAxis("Horizontal"); // перемещение по горизонтали
    //    transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, speed * Time.deltaTime);
    //    // текущее местоположение, место перемещения, скорость
    //    sprite.flipX = direction.x < 0.0f; // если направление < 0, то поворот влево
    //}

    private void Jump()
    {
        rigid_body.velocity = new Vector2(rigid_body.velocity.x, jump_force);
    }

    private void CheckGrounded()
    {
        grounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
    }

    private void Die()
    {
        is_dead = true;
        State = States.Death;

        rigid_body.velocity = Vector2.zero;
        rigid_body.bodyType = RigidbodyType2D.Kinematic;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }
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

        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(-100, deathY, 0), new Vector3(100, deathY, 0));
    }
}

public enum States // перечисление всех видов анимации
{
    Idle,
    Run,
    Jump,
    Death
}


