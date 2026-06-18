using UnityEngine;
using System.Collections;

public class WizardEnemy : Enemy
{
    [Header("Õ‡ÒÚÓÈÍË ÚÂÎÂÔÓÚ‡ˆËË")]
    [SerializeField] private Transform[] teleportPoints;
    [SerializeField] private float tpInterval = 15f;
    private int hitCounter = 0;

    [Header("Õ‡ÒÚÓÈÍË ÒÚÂÎ¸·˚")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 3f;
    [SerializeField] private float projectileSpeed = 5f;

    private Transform player;
    private float tpTimer;
    private float fireTimer;

    // Õ¿ÿ ÕŒ¬€… ‘À¿√-«¿Ÿ»“¿
    private bool isTeleporting = false;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        tpTimer = tpInterval;
        fireTimer = fireRate;
    }

    private void Update()
    {
        if (player == null) return;

        // ≈—À» Ã¿√ ¬ œ–Œ÷≈——≈ “≈À» ¿ ó œ–≈–€¬¿≈Ã UPDATE » ∆ƒ≈Ã!
        if (isTeleporting) return;

        // --- ÀŒ√» ¿ œŒ¬Œ–Œ“¿   »√–Œ ” ---
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = transform.position.x > player.position.x;
        }

        // --- “¿…Ã≈– “≈À≈œŒ–“¿ ---
        tpTimer -= Time.deltaTime;
        if (tpTimer <= 0)
        {
            Teleport();
        }

        // --- “¿…Ã≈– —“–≈À‹¡€ ---
        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0)
        {
            Shoot();
        }
    }

    public override void TakeDamage(int damageAmount)
    {
        // ≈ÒÎË Ï‡„ ÛÊÂ ÚÂÎÂÔÓÚËÛÂÚÒˇ, ÓÌ ÌÂ ‰ÓÎÊÂÌ ÔÓÎÛ˜‡Ú¸ ÛÓÌ Ë Á‡ÔÛÒÍ‡Ú¸ “œ Á‡ÌÓ‚Ó
        if (isTeleporting) return;

        base.TakeDamage(damageAmount);

        hitCounter++;
        if (hitCounter >= 3)
        {
            Teleport();
        }
    }

    private int lastPointIndex = -1;

    private void Teleport()
    {
        if (teleportPoints == null || teleportPoints.Length <= 1) return;
        if (isTeleporting) return; // «‡˘ËÚ‡ ÓÚ ‰‚ÓÈÌÓ„Ó ‚˚ÁÓ‚‡

        StartCoroutine(TeleportRoutine());
    }

    private IEnumerator TeleportRoutine()
    {
        if (spriteRenderer == null) yield break;

        isTeleporting = true; // ¡ÎÓÍËÛÂÏ Update Ë ÛÓÌ

        float duration = 0.4f;
        float timer = 0f;

        // --- 1. œÀ¿¬ÕŒ≈ »—◊≈«ÕŒ¬≈Õ»≈ ---
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = timer / duration;
            Color targetColor = Color.Lerp(Color.white, new Color(1f, 1f, 1f, 0f), progress);
            spriteRenderer.color = targetColor;
            yield return null;
        }

        // --- 2. œ≈–≈Ã≈Ÿ≈Õ»≈ Õ¿ ÕŒ¬”ﬁ “Œ◊ ” ---
        int randomIndex = lastPointIndex;
        while (randomIndex == lastPointIndex)
        {
            randomIndex = Random.Range(0, teleportPoints.Length);
        }
        lastPointIndex = randomIndex;

        transform.position = teleportPoints[randomIndex].position;

        // --- 3. œÀ¿¬ÕŒ≈ œŒﬂ¬À≈Õ»≈ ---
        timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = timer / duration;
            Color targetColor = Color.Lerp(new Color(1f, 1f, 1f, 0f), Color.white, progress);
            spriteRenderer.color = targetColor;
            yield return null;
        }

        spriteRenderer.color = Color.white;

        // —·‡Ò˚‚‡ÂÏ ‚ÒÂ Ú‡ÈÏÂ˚ Ë Ò˜ÂÚ˜ËÍË “ŒÀ‹ Œ ÔÓÒÎÂ ÔÓÎÌÓ„Ó ÔÓˇ‚ÎÂÌËˇ!
        hitCounter = 0;
        tpTimer = tpInterval;
        fireTimer = fireRate; // Œ·ÌÛÎˇÂÏ Ë Ú‡ÈÏÂ ÒÚÂÎ¸·˚, ˜ÚÓ·˚ ÓÌ ÌÂ ÒÚÂÎˇÎ Ò‡ÁÛ ÔÓÒÎÂ “œ

        isTeleporting = false; // ŒÚÍ˚‚‡ÂÏ Update, Ï‡„ ÒÌÓ‚‡ „ÓÚÓ‚ Í ·Ó˛!
    }

    private void Shoot()
    {
        if (projectilePrefab == null || firePoint == null || isTeleporting) return;

        GameObject ball = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Vector2 shootDirection = (player.position - firePoint.position).normalized;

        Projectile script = ball.GetComponent<Projectile>();
        if (script != null)
        {
            script.Launch(shootDirection, projectileSpeed);
        }

        fireTimer = fireRate;
    }
}