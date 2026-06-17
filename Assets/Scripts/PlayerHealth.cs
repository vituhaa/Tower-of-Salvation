using UnityEngine;
using UnityEngine.UI;
using System.Collections; // Обязательно для IEnumerator

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Настройки интерфейса")]
    [SerializeField] private Image[] hearts;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        currentHealth = maxHealth;
        // Автоматически находим компонент спрайта у Принца или его детей
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        UpdateHeartsUI();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHeartsUI();

        // Запускаем мигание при получении урона
        if (currentHealth > 0)
        {
            StartCoroutine(DamageFlashRoutine());
        }

        if (currentHealth <= 0)
        {
            GetComponent<Prince>().SendMessage("Die");
        }
    }

    private IEnumerator DamageFlashRoutine()
    {
        // 1. Окрашиваем в красный цвет
        spriteRenderer.color = Color.red;
        // Ждем 0.15 секунды
        yield return new WaitForSeconds(0.15f);

        // 2. Возвращаем обычный белый цвет (оригинальный вид спрайта)
        spriteRenderer.color = Color.white;
    }

    private void UpdateHeartsUI()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHealth)
                hearts[i].sprite = fullHeart;
            else
                hearts[i].sprite = emptyHeart;
        }
    }
}