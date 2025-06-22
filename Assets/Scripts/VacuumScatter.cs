using UnityEngine;
using System.Collections;

public class VacuumScatter : VacuumBehavior
{
    public float trackingInterval = 0.25f;
    private Coroutine trackCoroutine;

    private void OnEnable()
    {
        if (vacuum.isBoss)
            StartTrackingTarget();
    }

    private void OnDisable()
    {
        StopTrackingTarget();

        if (vacuum == null || vacuum.isExploded)
            return;

        if (vacuum.isBoss)
            vacuum.shooting?.Enable();
        else
            vacuum.chase?.Enable();
    }

    private void StartTrackingTarget()
    {
        if (trackCoroutine == null)
            trackCoroutine = StartCoroutine(TrackTargetRoutine());
    }

    private void StopTrackingTarget()
    {
        if (trackCoroutine != null)
        {
            StopCoroutine(trackCoroutine);
            trackCoroutine = null;
        }
    }

    private IEnumerator TrackTargetRoutine()
    {
        while (true)
        {
            if (vacuum != null && vacuum.target != null && !vacuum.frightened.enabled)
            {
                Vector2 direction = -(vacuum.target.position - transform.position).normalized;
                vacuum.movement.SetDirection(direction, forced: true);
            }

            yield return new WaitForSeconds(trackingInterval);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (vacuum == null || vacuum.isBoss || vacuum.frightened.enabled)
            return;

        Node node = other.GetComponent<Node>();
        if (node == null || !enabled) return;

        int index = Random.Range(0, node.availableDirections.Count);

        if (node.availableDirections[index] == -vacuum.movement.direction && node.availableDirections.Count > 1)
        {
            index = (index + 1) % node.availableDirections.Count;
        }

        vacuum.movement.SetDirection(node.availableDirections[index]);
    }
}
