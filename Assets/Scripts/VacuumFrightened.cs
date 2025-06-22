using UnityEngine;
using System.Collections;

public class VacuumFrightened : VacuumBehavior
{
    public SpriteRenderer body;
    public SpriteRenderer eyeWorking;
    public SpriteRenderer broken;
    public SpriteRenderer eyeBroken;
    public SpriteRenderer eyeFixing;
    private SpriteRenderer sphereRenderer;

    public float trackingInterval = 0.25f;

    public bool eaten { get; private set; }

    private Coroutine trackCoroutine;
    private Coroutine restoreCoroutine;

    public override void Enable(float duration)
    {
        if (enabled)
        {
            CancelInvoke(nameof(Flash));
            if (!vacuum.isBoss)
                Invoke(nameof(Flash), duration / 2.0f);
            base.Enable(duration); 
            return;
        }

        base.Enable(duration);

        body.enabled = false;
        eyeWorking.enabled = false;
        broken.enabled = true;
        eyeBroken.enabled = true;
        eyeFixing.enabled = false;

        if (vacuum.isBoss)
            vacuum.shooting.enabled = false;

        eaten = false;
        vacuum.movement.speedMultiplier = 0.5f;

        if (vacuum.isBoss)
            StartTrackingAway();

        if (!vacuum.isBoss)
            Invoke(nameof(Flash), duration / 2.0f);
    }

    public override void Disable()
    {
        base.Disable();

        StopTrackingAway();

        body.enabled = true;
        if (vacuum.isBoss)
        {
            eyeWorking.enabled = false;
            if (sphereRenderer != null)
                sphereRenderer.enabled = false;
        }
        else
            eyeWorking.enabled = true;
        broken.enabled = false;
        eyeBroken.enabled = false;
        eyeFixing.enabled = false;

        eaten = false;
        vacuum.movement.speedMultiplier = 1.0f;
    }

    private void Flash()
    {
        if (!eaten)
        {
            eyeBroken.enabled = false;
            eyeFixing.enabled = true;
            eyeFixing.GetComponent<AnimatedSprite>().Restart();
        }
    }

    private void Eaten()
    {
        if (restoreCoroutine != null)
        {
            StopCoroutine(restoreCoroutine);
        }
        eaten = true;

        Vector3 position;

        if (vacuum.isBoss)
        {
            position = vacuum.movement.startingPosition;
        }
        else
        {
            position = vacuum.home.inside.position;
        }

        position.z = vacuum.transform.position.z;
        vacuum.transform.position = position;

        if (!vacuum.isBoss)
        {
            vacuum.home.Enable(this.duration);
        }

        body.enabled = false;
        eyeWorking.enabled = true;
        broken.enabled = false;
        eyeBroken.enabled = false;
        eyeFixing.enabled = false;

        if (vacuum.isBoss)
        {
            GameObject sphere = vacuum.transform.Find("Sphere")?.gameObject;
            if (sphere != null)
            {
                sphereRenderer = sphere.GetComponent<SpriteRenderer>();
                if (sphereRenderer != null)
                    sphereRenderer.enabled = true;
            }
            Collider2D col = vacuum.GetComponent<Collider2D>();
            if (col != null)
                col.enabled = false;
            restoreCoroutine = StartCoroutine(RestoreBossAfterEaten(5f));
        }
    }

    private IEnumerator RestoreBossAfterEaten(float delay)
    {
        yield return new WaitForSeconds(delay);
        //body.enabled = true;

        Collider2D col = vacuum.GetComponent<Collider2D>();
        if (col != null)
            col.enabled = true;
        //if (sphereRenderer != null)
            //sphereRenderer.enabled = false;
        eaten = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (enabled && collision.gameObject.layer == LayerMask.NameToLayer("Cat"))
        {
            Eaten();
        }
    }

    private void StartTrackingAway()
    {
        if (trackCoroutine == null)
            trackCoroutine = StartCoroutine(TrackAwayRoutine());
    }

    private void StopTrackingAway()
    {
        if (trackCoroutine != null)
        {
            StopCoroutine(trackCoroutine);
            trackCoroutine = null;
        }
    }

    private IEnumerator TrackAwayRoutine()
    {
        while (true)
        {
            if (vacuum != null && vacuum.target != null)
            {
                Vector2 direction = -(vacuum.target.position - transform.position).normalized;
                vacuum.movement.SetDirection(direction, forced: true);
            }

            yield return new WaitForSeconds(trackingInterval);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (vacuum == null || vacuum.isBoss || !enabled)
            return;

        Node node = other.GetComponent<Node>();
        if (node == null) return;

        Vector2 direction = Vector2.zero;
        float maxDistance = float.MinValue;

        foreach (Vector2 availableDirection in node.availableDirections)
        {
            Vector3 newPosition = transform.position + new Vector3(availableDirection.x, availableDirection.y);
            float distance = (vacuum.target.position - newPosition).sqrMagnitude;

            if (distance > maxDistance)
            {
                direction = availableDirection;
                maxDistance = distance;
            }
        }

        vacuum.movement.SetDirection(direction);
    }
}
