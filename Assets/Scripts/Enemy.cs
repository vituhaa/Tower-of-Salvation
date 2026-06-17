using UnityEngine;
using System.Collections; // Обязательно для IEnumerator

public class Enemy : MonoBehaviour
{
    public int damage = 1;
    public int health = 2;

    [SerializeField] private SpriteRenderer spriteRenderer;
    private bool isFlashing = false;

    private void Start()
    {
        // Находим спрайт врага
        //spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }

    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;

        if (health > 0)
        {
            // Если враг еще жив, заставляем его мигать
            StartCoroutine(DamageFlashRoutine());
        }
        else
        {
            // Если умер — уничтожаем
            Destroy(gameObject);
        }
    }

    private IEnumerator DamageFlashRoutine()
    {
        if (isFlashing) yield break; // Защита от спама ударов
        isFlashing = true;

        // Окрашиваем врага в красный
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.15f);

        // Возвращаем исходный цвет
        spriteRenderer.color = Color.white;
        isFlashing = false;
    }
}