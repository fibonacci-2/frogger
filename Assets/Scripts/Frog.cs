using UnityEngine;

public class Frog : MonoBehaviour
{
    private float gridSize;
    private int frogGridX;
    private int frogGridY;

    void Start()
    {
        // Get grid size from GridManager
        if (GridManager.Instance != null)
            gridSize = GridManager.Instance.gridCellSize;
        
        // Initialize frog grid position and center it in its cell
        UpdateFrogGridPosition();
        CenterFrogInCell();
    }

    void Update()
    {
        // Only the player-controlled frog should read input and advance time
        if (!gameObject.CompareTag("Player"))
            return;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (TryMove(0, 1))
            {
                TickManager.Instance?.AdvanceTick();
                CheckWinCondition();
                CheckCollisionWithCars();
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (TryMove(0, -1))
            {
                TickManager.Instance?.AdvanceTick();
                CheckWinCondition();
                CheckCollisionWithCars();
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (TryMove(-1, 0))
            {
                TickManager.Instance?.AdvanceTick();
                CheckWinCondition();
                CheckCollisionWithCars();
            }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (TryMove(1, 0))
            {
                TickManager.Instance?.AdvanceTick();
                CheckWinCondition();
                CheckCollisionWithCars();
            }
        }
    }

    void UpdateFrogGridPosition()
    {
        // Convert world position to grid, accounting for cell center offset
        frogGridX = Mathf.RoundToInt((transform.position.x - gridSize * 0.5f) / gridSize);
        frogGridY = Mathf.RoundToInt((transform.position.y - gridSize * 0.5f) / gridSize);
    }

    void CenterFrogInCell()
    {
        // Position frog at the center of its grid cell
        transform.position = new Vector3(frogGridX * gridSize + gridSize * 0.5f, frogGridY * gridSize + gridSize * 0.5f, 0);
    }

    void CheckCollisionWithCars()
    {
        UpdateFrogGridPosition();
        
        if (GridManager.Instance != null)
        {
            Vector2Int frogCell = new Vector2Int(frogGridX, frogGridY);
            GameObject occupant = GridManager.Instance.GetOccupant(frogCell);
            
            // If cell is occupied by a car, pause the game
            if (occupant != null && occupant.CompareTag("car"))
                GameManager.Instance.Pause();
        }
    }

    void CheckWinCondition()
    {
        UpdateFrogGridPosition();
        
        if (GridManager.Instance != null)
        {
            Vector2Int frogCell = new Vector2Int(frogGridX, frogGridY);
            Vector2Int winSquare = GridManager.Instance.winningSquare;
            
            // If frog reached the winning square, notify GameManager
            if (frogCell == winSquare)
                GameManager.Instance.Win();
        }
    }

    bool TryMove(int deltaX, int deltaY)
    {
        int newGridX = frogGridX + deltaX;
        int newGridY = frogGridY + deltaY;
        Vector2Int newPosition = new Vector2Int(newGridX, newGridY);

        // Check if new position is valid (within bounds, not occupied, or is the winning square)
        if (!GridManager.Instance.CanMoveToPosition(newPosition))
        {
            return false; // Movement blocked, stay in place
        }

        // Move to new grid position
        frogGridX = newGridX;
        frogGridY = newGridY;
        CenterFrogInCell();
        return true;
    }
}
