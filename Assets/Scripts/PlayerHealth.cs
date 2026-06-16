using UnityEngine;
using UnityEngine.UI; // Обязательно для работы с Image

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Настройки сердечек")]
    [SerializeField] private Image[] hearts; // Массив объектов картинок сердечек
    [SerializeField] private Sprite fullHeart; // Текстура полного сердечка
    [SerializeField] private Sprite emptyHeart; // Текстура потраченного сердечка

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHeartsUI();
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("получаем урон!");
        currentHealth -= damage;

        // Ограничиваем здоровье, чтобы оно не ушло в минус
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHeartsUI();

        if (currentHealth <= 0)
        {
            GetComponent<Prince>().SendMessage("Die");
        }
    }

    private void UpdateHeartsUI()
    {
        // Проходимся циклом по всем сердечкам на экране
        for (int i = 0; i < hearts.Length; i++)
        {
            // Если индекс сердечка меньше текущего здоровья — оно полное
            if (i < currentHealth)
            {
                hearts[i].sprite = fullHeart;
            }
            else // Иначе — оно потраченное (пустое)
            {
                hearts[i].sprite = emptyHeart;
            }
        }
    }
}