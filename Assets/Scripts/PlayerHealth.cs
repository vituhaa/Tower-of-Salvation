using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Настройки интерфейса")]
    [SerializeField] private Image[] hearts;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;

    [Header("Настройки неуязвимости")]
    [SerializeField] private float damageCooldown = 1.0f; // Время неуязвимости в секундах
    private bool isInvincible = false; // Флаг: неуязвим ли игрок сейчас?

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        UpdateHeartsUI();
    }

    public void TakeDamage(int damage)
    {
        // ЕСЛИ ИГРОК НЕУЯЗВИМ — ИГНОРИРУЕМ УРОН
        if (isInvincible) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHeartsUI();

        if (currentHealth > 0)
        {
            // Получаем скрипт Принца и принудительно включаем анимацию Hurt
            Prince prince = GetComponent<Prince>();
            if (prince != null)
            {
                prince.PlayHurtAnimation();
            }

            // Запускаем корутину неуязвимости и мигания
            StartCoroutine(InvincibilityRoutine());
        }

        if (currentHealth <= 0)
        {
            GetComponent<Prince>().SendMessage("Die");
        }
    }

    private IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;

        float timer = 0f;
        // Чуть-чуть уменьшим интервал, чтобы мигание сочеталось с анимацией
        float flashInterval = 0.1f;

        // Даем анимации Hurt проиграться короткое время (например, 0.2 секунды),
        // прежде чем Принц снова сможет бегать и прыгать
        yield return new WaitForSeconds(0.2f);

        while (timer < damageCooldown - 0.2f)
        {
            spriteRenderer.color = (spriteRenderer.color == Color.white) ? Color.red : Color.white;
            yield return new WaitForSeconds(flashInterval);
            timer += flashInterval;
        }

        spriteRenderer.color = Color.white;
        isInvincible = false;
    }

    private void UpdateHeartsUI()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] == null) continue; // Защита от пустых слотов

            if (i < currentHealth)
                hearts[i].sprite = fullHeart;
            else
                hearts[i].sprite = emptyHeart;
        }
    }
}