using UnityEngine;

// Car moves one grid step when TickManager issues a tick.
public class Car : MonoBehaviour
{
    // Size of one grid cell (world units)
    public float gridSize = 1f;

    // How many ticks between each move (1 = move every tick)
    public int ticksPerMove = 1;

    // Direction: 1 = right, -1 = left
    public int direction = 1;

    int tickCounter = 0;

    void OnEnable()
    {
        if (TickManager.Instance != null)
            TickManager.Instance.OnTick += HandleTick;
    }

    void OnDisable()
    {
        if (TickManager.Instance != null)
            TickManager.Instance.OnTick -= HandleTick;
    }

    void HandleTick()
    {
        tickCounter++;
        if (tickCounter >= Mathf.Max(1, ticksPerMove))
        {
            transform.Translate(Vector3.right * direction * gridSize);
            tickCounter = 0;
        }
    }
}
