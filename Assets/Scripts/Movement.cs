using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    public float speed = 8.0f;
    public float speedMultiplier = 1.0f;
    public Vector2 initialDirection;
    public LayerMask obstacleLayer;

    public bool isBoss = false; 

    public new Rigidbody2D rigidbody { get; private set; }
    public Vector2 direction { get; private set; }
    public Vector2 nextDirection { get; private set; }
    public Vector3 startingPosition { get; private set; }

    private void Awake()
    {
        this.rigidbody = GetComponent<Rigidbody2D>();
        this.startingPosition = this.transform.position;
    }

    private void Start()
    {
        ResetState();
    }

    public void ResetState()
    {
        this.speedMultiplier = 1.0f;
        this.direction = this.initialDirection;
        this.nextDirection = Vector2.zero;
        this.transform.position = this.startingPosition;
        this.rigidbody.isKinematic = false;
        this.enabled = true;
    }

    private void Update()
    {
        if (nextDirection != Vector2.zero)
        {
            SetDirection(nextDirection);
        }
    }

    private void FixedUpdate()
    {
        if (Time.timeScale == 0f) return;

        if (this.nextDirection != Vector2.zero)
        {
            TrySetDirection(this.nextDirection);
        }

        Vector2 position = this.rigidbody.position;
        Vector2 translation = this.direction * this.speed * this.speedMultiplier * Time.fixedDeltaTime;
        this.rigidbody.MovePosition(position + translation);
    }

    public void SetDirection(Vector2 direction, bool forced = false)
    {
        if (isBoss || forced || !Occupied(direction))
        {
            this.direction = direction;
            this.nextDirection = Vector2.zero;
        }
        else
        {
            this.nextDirection = direction;
        }
    }

    private void TrySetDirection(Vector2 direction)
    {
        if (isBoss)
        {
            this.direction = direction;
            this.nextDirection = Vector2.zero;
            return;
        }

        Vector2 position = this.rigidbody.position;
        Vector2 offsetPosition = position + direction * 0.5f;

        RaycastHit2D hit = Physics2D.BoxCast(offsetPosition, Vector2.one * 0.75f, 0f, direction, 0.05f, this.obstacleLayer);

        if (hit.collider == null)
        {
            this.direction = direction;
            this.nextDirection = Vector2.zero;
        }
    }

    public bool Occupied(Vector2 direction)
    {
        if (isBoss) return false;

        Vector2 position = this.rigidbody.position;
        RaycastHit2D hit = Physics2D.BoxCast(position, Vector2.one * 0.75f, 0f, direction, 1.5f, this.obstacleLayer);
        return hit.collider != null;
    }
}
