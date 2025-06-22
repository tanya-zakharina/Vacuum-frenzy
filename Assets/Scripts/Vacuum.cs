using UnityEngine;

public class Vacuum : MonoBehaviour
{
    public Movement movement { get; private set; }
    public VacuumHome home { get; private set; }
    public VacuumScatter scatter { get; private set; }
    public VacuumChase chase { get; private set; }
    public VacuumFrightened frightened { get; private set; }
    public VacuumShoot shooting { get; private set; }

    public VacuumBehavior initialBehavior;
    [HideInInspector] public Transform target;
    private bool isPaused;
    public bool isBoss = false;
    public bool isExploded = false;

    public int points = 200;

    private void Awake()
    {
        this.movement = GetComponent<Movement>();
        this.home = GetComponent<VacuumHome>();
        this.scatter = GetComponent<VacuumScatter>();
        this.chase = GetComponent<VacuumChase>();
        this.frightened = GetComponent<VacuumFrightened>();

        if (CompareTag("Boss"))
        {
            this.shooting = GetComponent<VacuumShoot>();
        }
    }

    private void Start()
    {
        ResetState();
    }

    private void FixedUpdate()
    {
        UpdateRotation();
    }

    public void UpdateRotation()
    {
        if (movement.direction != Vector2.zero)
        {
            float angle = Mathf.Atan2(movement.direction.y, movement.direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    public void ForceUpdateRotation()
    {
        UpdateRotation();
    }

    public void ResetState()
    {
        this.gameObject.SetActive(true);
        this.movement.ResetState();

        this.frightened.Disable();
        this.chase.Disable();
        if (isExploded) return;
        else this.scatter.Enable();

        if (!CompareTag("Boss"))
        {
            this.scatter.Enable();

            if (this.home != null && this.home != this.initialBehavior)
            {
                this.home.Disable();
            }

            if (this.initialBehavior != null)
            {
                this.initialBehavior.Enable();
            }
        }
        else
        {
            transform.position = this.movement.startingPosition;
            this.scatter.Enable(); 
        }

        if (this.initialBehavior != null)
        {
            this.initialBehavior.Enable();
        }

        if (this.shooting != null)
        {
            this.shooting.Disable(); 
        }

        ForceUpdateRotation();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Cat"))
        {
            if (frightened.enabled)
            {
                GameManager.Instance.VacuumEaten(this);
            }
            else
            {
                GameManager.Instance.CatEaten();
            }
        }
    }

    public void SetTarget(Transform catTransform)
    {
        this.target = catTransform;
    }

    public void DisableAllBehaviors()
    {
        if (scatter != null) scatter.enabled = false;
        if (chase != null) chase.enabled = false;
        if (frightened != null) frightened.enabled = false;
        if (shooting != null) shooting.enabled = false;
    }
}
