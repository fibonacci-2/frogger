using UnityEngine;

// Car moves one grid step when TickManager issues a tick.
public class Car : MonoBehaviour
{
    public float gridSize = 16f;
    public int ticksPerMove = 1;
    public int direction = 1; // 1 = right, -1 = left

    private int tickCounter = 0;
    private int gridX;
    private int gridY;

    void Start()
    {
        // Initialize grid position from world position
        // Assumes world position is already grid-aligned (gridX * gridSize, gridY * gridSize)
        gridX = Mathf.RoundToInt(transform.position.x / gridSize);
        gridY = Mathf.RoundToInt(transform.position.y / gridSize);
        
        Debug.Log($"Car spawned at grid ({gridX}, {gridY})");
        
        // Register in grid
        if (GridManager.Instance != null)
        {
            if (!GridManager.Instance.TryOccupy(new Vector2Int(gridX, gridY), gameObject))
            {
                Debug.LogWarning($"Failed to occupy grid ({gridX}, {gridY})");
                Destroy(gameObject);
            }
        }
        
        // Ensure position is exactly on grid
        SetGridPosition(gridX, gridY);
    }

    void OnEnable()
    {
        if (TickManager.Instance != null)
            TickManager.Instance.OnTick += HandleTick;
    }

    void OnDisable()
    {
        if (TickManager.Instance != null)
            TickManager.Instance.OnTick -= HandleTick;
        
        // Release grid square when destroyed
        if (GridManager.Instance != null)
            GridManager.Instance.Release(new Vector2Int(gridX, gridY));
    }

    void HandleTick()
    {
        tickCounter++;
        if (tickCounter >= Mathf.Max(1, ticksPerMove))
        {
            TryMove();
            tickCounter = 0;
        }
    }

    void TryMove()
    {
        int newGridX = gridX + direction;

        // Check if new position is within bounds
        if (!GridManager.Instance.IsWithinBounds(new Vector2Int(newGridX, gridY)))
        {
            Debug.Log($"Car reached end of grid at ({gridX}, {gridY})");
            Destroy(gameObject);
            return;
        }

        // Try to move
        if (GridManager.Instance.TryMove(new Vector2Int(gridX, gridY), new Vector2Int(newGridX, gridY), gameObject))
        {
            gridX = newGridX;
            SetGridPosition(gridX, gridY);
        }
        // If occupied, just wait for next tick
    }

    void SetGridPosition(int x, int y)
    {
        // Set position to exact grid square center
        transform.position = new Vector3(x * gridSize + gridSize * 0.5f, y * gridSize + gridSize * 0.5f, 0);
    }
}


