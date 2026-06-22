using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private float currentHealth;

    [SerializeField] private Image[] hearts;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;
    [SerializeField] private float damageCooldown = 1.0f; // врем€ неу€звимости в секундах
    private bool isInvincible = false; // флаг неу€звим ли игрок сейчас

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        UpdateHeartsUI();
    }

    public void TakeDamage(float damage)
    {
        if (isInvincible) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHeartsUI();

        if (currentHealth > 0)
        {
            Prince prince = GetComponent<Prince>();
            if (prince != null)
            {
                prince.PlayHurtAnimation();
            }

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
        float flashInterval = 0.1f;

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
            if (hearts[i] == null) continue; 

            if (i < currentHealth)
                hearts[i].sprite = fullHeart;
            else
                hearts[i].sprite = emptyHeart;
        }
    }
}