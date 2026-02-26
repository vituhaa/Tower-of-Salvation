using UnityEngine;
using System.Collections;

public class Prince : MonoBehaviour
{
    private float speed = 3f; // скорость движени€
    private float jump_force = 14f; // сила прыжка
    private bool grounded = false;

    private Rigidbody2D rigid_body; // ссылка на компонент
    private Animator animations; // ссылка на компонент анимации
    private SpriteRenderer sprite; // ссылка на компонент где изображение принца
    private AudioSource audioSource; // ссылка на компонент AudioSource

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

    private void FixedUpdate()
    {
        CheckGrounded();
    }
    void Update()
    {
        if (grounded)
        {
            State = States.Idle; // состо€ние поко€
        }

        if (Input.GetButton("Horizontal"))
        {
            Run();
        }
        if (grounded)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow)) // проверка на нажатие пробела или стрелки вверх
            {
                Jump();
            }
        }
    }

    private void Run()
    {
        if (grounded)
        {
            State = States.Run; // состо€ние бега
        }
        Vector3 direction = transform.right * Input.GetAxis("Horizontal"); // перемещение по горизонтали
        transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, speed * Time.deltaTime);
        // текущее местоположение, место перемещени€, скорость
        sprite.flipX = direction.x < 0.0f; // если направление < 0, то поворот влево
    }

    private void Jump()
    {
        rigid_body.AddForce(transform.up * jump_force, ForceMode2D.Impulse);
    }

    private void CheckGrounded()
    {
        grounded = Mathf.Abs(rigid_body.velocity.y) < 0.01f;

        if (!grounded)
        {
            State = States.Jump;
        }
        //Collider2D[] collider = Physics2D.OverlapBoxAll(transform.position, new Vector2(1.4f, 0.3f), 0f); // массив коллайдеров
        //grounded = collider.Length > 1; // если есть коллайдер под ногами, то мы на земле
        //if (!grounded)
        //{
        //    State = States.Jump; // если не стоим на земле - прыгаем
        //}
    }

    //// Ќовый метод дл€ обработки столкновений
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("enemy"))
    //    {
    //        if (audioSource != null)
    //        {
    //            audioSource.Stop(); // ќстанавливаем предыдущий звук (если нужно)
    //            audioSource.Play(); // ѕроигрываем звук снова
    //        }
    //        else
    //        {
    //            Debug.Log("AudioSource не найден.");
    //        }
    //    }
    //}
}

public enum States // перечисление всех видов анимации
{
    Idle,
    Run,
    Jump
}