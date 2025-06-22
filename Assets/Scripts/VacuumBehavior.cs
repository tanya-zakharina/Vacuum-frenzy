using UnityEngine;

[RequireComponent (typeof(Vacuum))]
public abstract class VacuumBehavior : MonoBehaviour
{
    public Vacuum vacuum {  get; private set; }
    public float duration;

    private void Awake()
    {
        this.vacuum = GetComponent<Vacuum> ();
        this.enabled = false;
    }

    public void Enable()
    {
         Enable(this.duration);
    }

    public virtual void Enable(float duration)
    {
        if (vacuum.isExploded)
            return;

        this.enabled = true;

        CancelInvoke();
        Invoke(nameof(Disable), duration);
    }

    public virtual void Disable()
    {
        this.enabled = false;

        CancelInvoke();
    }

}
