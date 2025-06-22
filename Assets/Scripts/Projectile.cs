using UnityEngine;
using UnityEngine.Audio;

public class Projectile : MonoBehaviour
{
    public float speed = 5f;
    public float lifetime = 5f;
    public AudioClip shootSound;

    public AudioMixerGroup sfxGroup; 

    private Vector2 direction;
    private AudioSource audioSource;

    public void Initialize(Transform target)
    {
        direction = (target != null)
            ? (target.position - transform.position).normalized
            : Vector2.right;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        PlayShootSound();
        Destroy(gameObject, lifetime);
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.outputAudioMixerGroup = sfxGroup;
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    private void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Cat cat = other.GetComponent<Cat>();
        if (cat != null)
        {
            GameManager.Instance.CatEaten();
            Destroy(gameObject);
            return;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            Destroy(gameObject);
        }
    }

    private void PlayShootSound()
    {
        if (shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSound);
        }
    }

    public static void DestroyAllProjectiles()
    {
        Projectile[] allProjectiles = FindObjectsOfType<Projectile>();
        foreach (var projectile in allProjectiles)
        {
            Destroy(projectile.gameObject);
        }
    }
}
