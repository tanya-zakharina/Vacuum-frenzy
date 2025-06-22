using System.Collections;
using UnityEngine;

public class VacuumHome : VacuumBehavior
{
    public Transform inside;
    public Transform outside;

    private void OnEnable()
    {
        StopAllCoroutines();
    }

    private void OnDisable()
    {
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(ExitTransition());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (enabled && collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            vacuum.movement.SetDirection(-vacuum.movement.direction);
        }
    }

    private IEnumerator ExitTransition()
    {
        vacuum.movement.SetDirection(Vector2.up, true);
        vacuum.movement.rigidbody.bodyType = RigidbodyType2D.Kinematic;
        vacuum.movement.enabled = false;

        Vector3 position = this.transform.position;

        float duration = 0.5f;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            Vector3 newPosition = Vector3.Lerp(position, inside.position, elapsed/duration);
            newPosition.z = position.z;
            vacuum.transform.position = newPosition;
            elapsed += Time.deltaTime;
            yield return null;
        }

        position = inside.position;
        elapsed = 0.0f;

        while (elapsed < duration)
        {
            Vector3 newPosition = Vector3.Lerp(position, outside.position, elapsed / duration);
            newPosition.z = position.z;
            vacuum.transform.position = newPosition;
            elapsed += Time.deltaTime;
            yield return null;
        }


        vacuum.movement.SetDirection(new Vector2(Random.value < 0.5f ? -1.0f : 1.0f, 0.0f), true);
        vacuum.movement.rigidbody.bodyType = RigidbodyType2D.Dynamic;
        vacuum.movement.enabled = true;
    }
}
