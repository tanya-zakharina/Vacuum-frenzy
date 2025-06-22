using UnityEngine;
using System.Collections;

public class VacuumShoot : VacuumBehavior
{
    public SpriteRenderer body;
    public SpriteRenderer broken;
    public SpriteRenderer shooting;

    public GameObject projectilePrefab;
    public Transform firePoint;
    public float shootInterval = 1.5f;
    public float trackingInterval = 0.25f;

    private Coroutine shootCoroutine;
    private Coroutine trackCoroutine;

    private bool wasFrightened = false;
    public static bool AllowShooting = true;

    private void Update()
    {
        if (vacuum == null)
            return;

        bool currentlyFrightened = vacuum.frightened.enabled;

        // Если frightened выключился — восстанавливаем состояние
        if (wasFrightened && !currentlyFrightened)
        {
            ResetStateAfterFrightened();
        }

        wasFrightened = currentlyFrightened;

        // Включаем этот скрипт, если frightened выключен, но скрипт ещё не активен
        if (!currentlyFrightened && !enabled)
        {
            enabled = true;
        }

        
    }

    private void OnEnable()
    {
        if (vacuum == null || vacuum.isExploded || vacuum.frightened.enabled)
            return;

        SetSpritesForShooting();
        StartShooting();

        if (vacuum.isBoss)
            StartTrackingBoss();
    }

    private void OnDisable()
    {
        ResetSprites();
        StopShooting();

        if (vacuum.isBoss)
            StopTrackingBoss();

        if (vacuum == null || vacuum.isExploded)
            return;

        vacuum.scatter?.Enable();
    }

    private void SetSpritesForShooting()
    {
        if (body != null) body.enabled = false;
        if (broken != null) broken.enabled = false;

        if (shooting != null)
        {
            shooting.enabled = true;
            var anim = shooting.GetComponent<AnimatedSprite>();
            if (anim != null)
                anim.Restart();
        }
    }

    private void ResetSprites()
    {
        if (vacuum == null || vacuum.isExploded)
            return;

        //if (body != null) body.enabled = false;
        //if (broken != null) broken.enabled = true;
        if (shooting != null) shooting.enabled = false;
    }

    public void StartShooting()
    {
        if (shootCoroutine == null)
        {
            shootCoroutine = StartCoroutine(ShootRoutine());
        }
    }

    public void StopShooting()
    {
        if (shootCoroutine != null)
        {
            StopCoroutine(shootCoroutine);
            shootCoroutine = null;
        }
    }

    private IEnumerator ShootRoutine()
    {
        while (true)
        {
            Shoot();
            yield return new WaitForSeconds(shootInterval);
        }
    }

    private void Shoot()
    {
        if (!AllowShooting)
            return;
        if (projectilePrefab == null || firePoint == null || vacuum == null)
            return;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile projScript = projectile.GetComponent<Projectile>();
        if (projScript != null)
        {
            projScript.Initialize(vacuum.target);
        }

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 direction = (vacuum.target != null)
                ? (vacuum.target.position - firePoint.position).normalized
                : Vector2.right;

            rb.linearVelocity = direction * 5f;
        }
    }

    private void StartTrackingBoss()
    {
        if (trackCoroutine == null)
            trackCoroutine = StartCoroutine(TrackTarget());
    }

    private void StopTrackingBoss()
    {
        if (trackCoroutine != null)
        {
            StopCoroutine(trackCoroutine);
            trackCoroutine = null;
        }
    }

    private IEnumerator TrackTarget()
    {
        while (true)
        {
            if (vacuum != null && vacuum.target != null && !vacuum.frightened.enabled)
            {
                Vector2 direction = (vacuum.target.position - transform.position).normalized;
                vacuum.movement.SetDirection(direction, forced: true);
            }

            yield return new WaitForSeconds(trackingInterval);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (vacuum == null || vacuum.frightened.enabled || vacuum.target == null || vacuum.isBoss)
            return;

        Node node = other.GetComponent<Node>();
        if (node == null || !enabled)
            return;

        Vector2 direction = Vector2.zero;
        float minDistance = float.MaxValue;

        foreach (Vector2 availableDirection in node.availableDirections)
        {
            Vector3 newPosition = transform.position + new Vector3(availableDirection.x, availableDirection.y);
            float distance = (vacuum.target.position - newPosition).sqrMagnitude;

            if (distance < minDistance)
            {
                direction = availableDirection;
                minDistance = distance;
            }
        }

        vacuum.movement.SetDirection(direction);
    }

    public void ResetStateAfterFrightened()
    {
        if (vacuum == null || vacuum.isExploded || vacuum.frightened.enabled)
            return;

        SetSpritesForShooting();
        StartShooting();

        if (vacuum.isBoss)
            StartTrackingBoss();
    }
}
