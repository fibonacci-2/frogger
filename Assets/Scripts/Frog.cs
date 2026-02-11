using UnityEngine;

public class Frog : MonoBehaviour
{
    public enum ControlScheme
    {
        Arrows,
        WASD
    }

    [SerializeField] private ControlScheme controlScheme = ControlScheme.Arrows;
    [SerializeField] private bool isPlayerControlled = true;
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
        // Only player-controlled frogs should read input and advance time
        if (!isPlayerControlled)
            return;

        GetMovementKeys(out KeyCode upKey, out KeyCode downKey, out KeyCode leftKey, out KeyCode rightKey);

        if (Input.GetKeyDown(upKey))
        {
            if (TryMove(0, 1))
            {
                TickManager.Instance?.AdvanceTick();
                CheckWinCondition();
                CheckCollisionWithCars();
            }
        }
        if (Input.GetKeyDown(downKey))
        {
            if (TryMove(0, -1))
            {
                TickManager.Instance?.AdvanceTick();
                CheckWinCondition();
                CheckCollisionWithCars();
            }
        }
        if (Input.GetKeyDown(leftKey))
        {
            if (TryMove(-1, 0))
            {
                TickManager.Instance?.AdvanceTick();
                CheckWinCondition();
                CheckCollisionWithCars();
            }
        }
        if (Input.GetKeyDown(rightKey))
        {
            if (TryMove(1, 0))
            {
                TickManager.Instance?.AdvanceTick();
                CheckWinCondition();
                CheckCollisionWithCars();
            }
        }
    }

    void GetMovementKeys(out KeyCode upKey, out KeyCode downKey, out KeyCode leftKey, out KeyCode rightKey)
    {
        if (controlScheme == ControlScheme.WASD)
        {
            upKey = KeyCode.W;
            downKey = KeyCode.S;
            leftKey = KeyCode.A;
            rightKey = KeyCode.D;
            return;
        }

        upKey = KeyCode.UpArrow;
        downKey = KeyCode.DownArrow;
        leftKey = KeyCode.LeftArrow;
        rightKey = KeyCode.RightArrow;
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
            Vector2Int winSquare = GridManager.Instance.GetWinningSquare(controlScheme);
            
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

        // Check if new position is valid (within bounds, not occupied, or is the frog's winning square)
        if (!GridManager.Instance.CanMoveToPosition(newPosition, controlScheme))
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
