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
            // Запускаем корутину неуязвимости и мигания
            StartCoroutine(InvincibilityRoutine());
        }

        if (currentHealth <= 0)
        {
            GetComponent<Prince>().SendMessage("Die");
        }
    }

    // Корутина, которая защищает игрока и заставляет его мигать
    private IEnumerator InvincibilityRoutine()
    {
        isInvincible = true; // Включаем защиту

        // Сделаем красивое геймерское мигание (вкл/выкл красный цвет), пока идет кулдаун
        float timer = 0f;
        float flashInterval = 0.15f; // Скорость мигания

        while (timer < damageCooldown)
        {
            // Переключаем цвет: если белый — делаем красным, если красный — возвращаем белый
            spriteRenderer.color = (spriteRenderer.color == Color.white) ? Color.red : Color.white;

            yield return new WaitForSeconds(flashInterval);
            timer += flashInterval;
        }

        // Гарантированно возвращаем Принцу нормальный цвет после кулдауна
        spriteRenderer.color = Color.white;
        isInvincible = false; // Выключаем защиту, теперь его снова можно ранить
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