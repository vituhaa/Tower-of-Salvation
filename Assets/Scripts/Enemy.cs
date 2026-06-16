using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int damage = 1; // Сколько урона наносит враг
    public int health = 2;  // Здоровье врага (умрет от 2 ударов)

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Если враг врезался в игрока
        if (collision.gameObject.CompareTag("Player"))
        {
            // Ищем у игрока скрипт здоровья и наносим урон
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

        if (health <= 0)
        {
            Destroy(gameObject); // Враг исчезает (умирает)
        }
    }
}