using UnityEngine;

public class PowerPellet : Pellet
{
    public float duration = 5.0f;

    protected override void Eat()
    {
        FindFirstObjectByType<GameManager>().PowerPelletEaten(this);
    }
}
