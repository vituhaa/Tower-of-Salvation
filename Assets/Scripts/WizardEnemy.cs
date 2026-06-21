using UnityEngine;
using System.Collections;

public class WizardEnemy : Enemy
{
    [Header("Настройки телепортации")]
    [SerializeField] private Transform[] teleportPoints;
    [SerializeField] private float tpInterval = 15f;
    private int hitCounter = 0;

    [Header("Настройки стрельбы")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 3f;
    [SerializeField] private float projectileSpeed = 5f;

    private Transform player;
    private float tpTimer;
    private float fireTimer;

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

        if (isTeleporting) return;

        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = transform.position.x > player.position.x;
        }

        tpTimer -= Time.deltaTime;
        if (tpTimer <= 0)
        {
            Teleport();
        }

        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0)
        {
            Shoot();
        }
    }

    public override void TakeDamage(int damageAmount)
    {
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
        if (isTeleporting) return; 

        StartCoroutine(TeleportRoutine());
    }

    private IEnumerator TeleportRoutine()
    {
        if (spriteRenderer == null) yield break;

        isTeleporting = true; 

        float duration = 0.4f;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = timer / duration;
            Color targetColor = Color.Lerp(Color.white, new Color(1f, 1f, 1f, 0f), progress);
            spriteRenderer.color = targetColor;
            yield return null;
        }

        int randomIndex = lastPointIndex;
        while (randomIndex == lastPointIndex)
        {
            randomIndex = Random.Range(0, teleportPoints.Length);
        }
        lastPointIndex = randomIndex;

        transform.position = teleportPoints[randomIndex].position;

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

        hitCounter = 0;
        tpTimer = tpInterval;
        fireTimer = fireRate;

        isTeleporting = false;
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