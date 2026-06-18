using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public int health = 2;  // Общее здоровье для любого врага
    [SerializeField] public SpriteRenderer spriteRenderer;
    private bool isFlashing = false;

    private void Start()
    {
        // Если забыли привязать в инспекторе, ищем в самом объекте
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public virtual void TakeDamage(int damageAmount)
    {
        health -= damageAmount;

        if (health > 0)
        {
            StartCoroutine(DamageFlashRoutine());
        }
        else
        {
            Die();
        }
    }

    private IEnumerator DamageFlashRoutine()
    {
        if (isFlashing) yield break;
        isFlashing = true;

        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.15f);

        spriteRenderer.color = Color.white;
        isFlashing = false;
    }

    private void Die()
    {
        // Здесь в будущем можно включить анимацию смерти или запустить частицы
        Destroy(gameObject);
    }
}