using UnityEngine;
using System.Collections.Generic;

// Manages an 8x8 grid and tracks which squares are occupied
public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }
    
    private const int GRID_SIZE = 8;
    public float gridCellSize = 16f;
    public Vector2Int winningSquare = new Vector2Int(3, 7);  // Universal winning square
    public Dictionary<Vector2Int, GameObject> occupiedSquares = new Dictionary<Vector2Int, GameObject>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    /// <summary>
    /// Tries to occupy a grid square. Returns true if successful, false if occupied.
    /// </summary>
    public bool TryOccupy(Vector2Int gridPosition, GameObject occupant)
    {
        if (!IsWithinBounds(gridPosition))
            return false;

        if (occupiedSquares.ContainsKey(gridPosition))
            return false;

        occupiedSquares[gridPosition] = occupant;
        return true;
    }

    /// <summary>
    /// Releases a grid square (e.g., when an object moves or is destroyed).
    /// </summary>
    public void Release(Vector2Int gridPosition)
    {
        occupiedSquares.Remove(gridPosition);
    }

    /// <summary>
    /// Moves an object from one grid square to another.
    /// Returns true if successful, false if destination is occupied.
    /// </summary>
    public bool TryMove(Vector2Int fromPosition, Vector2Int toPosition, GameObject occupant)
    {
        if (!IsWithinBounds(toPosition))
            return false;

        // Check if destination is occupied
        if (occupiedSquares.ContainsKey(toPosition))
            return false;

        // Move the occupant
        Release(fromPosition);
        occupiedSquares[toPosition] = occupant;
        return true;
    }

    /// <summary>
    /// Checks if a grid position is within the 8x8 grid.
    /// </summary>
    public bool IsWithinBounds(Vector2Int gridPosition)
    {
        return gridPosition.x >= 0 && gridPosition.x < GRID_SIZE &&
               gridPosition.y >= 0 && gridPosition.y < GRID_SIZE;
    }

    /// <summary>
    /// Gets what's occupying a grid square, or null if empty.
    /// </summary>
    public GameObject GetOccupant(Vector2Int gridPosition)
    {
        occupiedSquares.TryGetValue(gridPosition, out var occupant);
        return occupant;
    }

    /// <summary>
    /// Converts world position to grid position.
    /// </summary>
    public Vector2Int WorldToGrid(Vector3 worldPosition, float gridCellSize)
    {
        return new Vector2Int(
            Mathf.RoundToInt(worldPosition.x / gridCellSize),
            Mathf.RoundToInt(worldPosition.y / gridCellSize)
        );
    }

    /// <summary>
    /// Converts grid position to world position.
    /// </summary>
    public Vector3 GridToWorld(Vector2Int gridPosition, float gridCellSize)
    {
        return new Vector3(gridPosition.x * gridCellSize, gridPosition.y * gridCellSize, 0);
    }

    /// <summary>
    /// Gets the world position of a specific grid cell's center.
    /// </summary>
    public Vector3 GetGridCellCenter(Vector2Int gridPosition, float gridCellSize)
    {
        return GridToWorld(gridPosition, gridCellSize) + new Vector3(gridCellSize * 0.5f, gridCellSize * 0.5f, 0);
    }

    /// <summary>
    /// Validates if a position is valid for movement (within bounds and not occupied, or is the winning square).
    /// The winning square is always accessible as an exception to normal rules.
    /// </summary>
    public bool CanMoveToPosition(Vector2Int gridPosition)
    {
        // Winning square is always accessible (exception to normal rules)
        if (gridPosition == winningSquare)
            return true;

        // Must be within bounds
        if (!IsWithinBounds(gridPosition))
            return false;

        // Must not be occupied by another object
        if (occupiedSquares.ContainsKey(gridPosition))
            return false;

        return true;
    }
}