using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Movement))]
public class Cat : MonoBehaviour
{
    [SerializeField]
    private AnimatedSprite deathSequence;
    public AnimatedSprite winSequence;
    private SpriteRenderer spriteRenderer;
    private CircleCollider2D circleCollider;
    private Movement movement;

    private Vector2 startTouchPos;
    private Vector2 endTouchPos;
    private bool swipeHandled = false;
    public Rigidbody2D rb;
    public AnimatedSprite walking;


    private void Awake()
    {
        this.movement = GetComponent<Movement>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        this.circleCollider = GetComponent<CircleCollider2D>();
    }
    void Start()
    {
        GameManager.Instance?.RegisterCat(this);

        Vacuum[] vacuums = FindObjectsOfType<Vacuum>();
        foreach (var vacuum in vacuums)
        {
            vacuum.SetTarget(transform);
        }
    }

    private void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        var keyboard = Keyboard.current;

        if (keyboard != null)
        {
            if (keyboard.wKey.wasPressedThisFrame || keyboard.upArrowKey.wasPressedThisFrame)
            {
                movement.SetDirection(Vector2.up);
            }
            else if (keyboard.sKey.wasPressedThisFrame || keyboard.downArrowKey.wasPressedThisFrame)
            {
                movement.SetDirection(Vector2.down);
            }
            else if (keyboard.aKey.wasPressedThisFrame || keyboard.leftArrowKey.wasPressedThisFrame)
            {
                movement.SetDirection(Vector2.left);
            }
            else if (keyboard.dKey.wasPressedThisFrame || keyboard.rightArrowKey.wasPressedThisFrame)
            {
                movement.SetDirection(Vector2.right);
            }
        }
#endif

        var touchscreen = Touchscreen.current;
        if (touchscreen != null && touchscreen.touches.Count > 0)
        {
            var touch = touchscreen.primaryTouch;

            if (touch.press.wasPressedThisFrame)
            {
                startTouchPos = touch.position.ReadValue();
                swipeHandled = false;
            }
            else if (touch.press.wasReleasedThisFrame && !swipeHandled)
            {
                Vector2 endTouchPos = touch.position.ReadValue();
                Vector2 delta = endTouchPos - startTouchPos;

                if (delta.magnitude > 50f)
                {
                    swipeHandled = true;

                    if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                    {
                        movement.SetDirection(delta.x > 0 ? Vector2.right : Vector2.left);
                    }
                    else
                    {
                        movement.SetDirection(delta.y > 0 ? Vector2.up : Vector2.down);
                    }
                }
            }
        }

        if (movement.direction.x < 0)
        {
            walking.transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (movement.direction.x > 0)
        {
            walking.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void ResetState()
    {
        deathSequence.gameObject.SetActive(false);
        winSequence.gameObject.SetActive(false);
        enabled = true;
        spriteRenderer.enabled = true;
        circleCollider.enabled = true;
        deathSequence.enabled = false;
        winSequence.enabled = false;
        movement.ResetState();
        gameObject.SetActive(true);
    }

    public void DeathSequence()
    {
        deathSequence.gameObject.SetActive(true);
        enabled = false;
        spriteRenderer.enabled = false;
        circleCollider.enabled = false;
        movement.enabled = false;
        deathSequence.enabled = true;
        deathSequence.Restart();
    }

    public void WinSequence()
    {
        winSequence.gameObject.SetActive(true);
        enabled = false;
        spriteRenderer.enabled = false;
        circleCollider.enabled = false;
        movement.enabled = false;
        winSequence.enabled = true;
        winSequence.Restart();
    }
}
