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
            transform.Translate(Vector3.up * gridSize);
            TickManager.Instance?.AdvanceTick();
            CheckCollisionWithCars();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            transform.Translate(Vector3.down * gridSize);
            TickManager.Instance?.AdvanceTick();
            CheckCollisionWithCars();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.Translate(Vector3.left * gridSize);
            TickManager.Instance?.AdvanceTick();
            CheckCollisionWithCars();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.Translate(Vector3.right * gridSize);
            TickManager.Instance?.AdvanceTick();
            CheckCollisionWithCars();
        }
    }

    void UpdateFrogGridPosition()
    {
        // Convert world position to grid, accounting for cell center offset
        frogGridX = Mathf.RoundToInt((transform.position.x - gridSize * 0.5f) / gridSize);
        frogGridY = Mathf.RoundToInt((transform.position.y - gridSize * 0.5f) / gridSize);
        Debug.Log($"Frog grid position updated to ({frogGridX}, {frogGridY})");
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
}
