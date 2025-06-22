using UnityEngine;
using UnityEngine.Audio;

public class BossExplosion : MonoBehaviour
{
    public AudioMixerGroup sfxGroup;

    public SpriteRenderer normalVisual;
    public SpriteRenderer brokenVisual;
    public SpriteRenderer shootingVisual;
    public SpriteRenderer frightenedVisual;
    public AnimatedSprite explosionAnimation;
    public AudioSource audioSource;
    public AudioClip explosionClip;

    private Movement movement;
    private bool exploded = false;

    private void Awake()
    {
        movement = GetComponent<Movement>();
    }

    private void Update()
    {
        if (!exploded && GameManager.Instance != null && GameManager.Instance.AllPelletsEaten())
        {
            Explode();
        }
    }

    private void Explode()
    {
        exploded = true;

        if (movement != null)
        {
            movement.enabled = false;
            if (movement.rigidbody != null)
            {
                movement.rigidbody.linearVelocity = Vector2.zero;
            }
        }

        if (normalVisual != null)
            normalVisual.enabled = false;

        if (brokenVisual != null)
            brokenVisual.enabled = false;

        if (shootingVisual != null)
        {
            shootingVisual.enabled = false;

            AnimatedSprite anim = shootingVisual.GetComponent<AnimatedSprite>();
            if (anim != null)
                anim.enabled = false;
        }

        if (frightenedVisual != null)
            frightenedVisual.enabled = false;

        Vacuum vacuum = GetComponent<Vacuum>();
        if (vacuum != null)
        {
            vacuum.isExploded = true;
            vacuum.DisableAllBehaviors();
        }

        if (explosionAnimation != null)
        {
            explosionAnimation.gameObject.SetActive(true);
            explosionAnimation.Restart();
        }

        if (audioSource != null && explosionClip != null)
        {
            audioSource.outputAudioMixerGroup = sfxGroup;
            audioSource.PlayOneShot(explosionClip);
        }    
            
    }

    private void OnEnable()
    {
        if (normalVisual != null)
            normalVisual.enabled = false;

        if (brokenVisual != null)
            brokenVisual.enabled = false;

        if (shootingVisual != null)
            shootingVisual.enabled = false;

        if (frightenedVisual != null)
            frightenedVisual.enabled = false;

        if (explosionAnimation != null)
            explosionAnimation.gameObject.SetActive(false);
    }
}
