using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }
    
    private const int GRID_SIZE = 8;
    public float gridCellSize = 16f;
    public Vector2Int[] winningSquares =
    {
        new Vector2Int(4, 8),
        new Vector2Int(3, 8)
    };
    public Dictionary<Vector2Int, GameObject> occupiedSquares = new Dictionary<Vector2Int, GameObject>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    public bool TryOccupy(Vector2Int gridPosition, GameObject occupant)
    {
        if (!IsWithinBounds(gridPosition))
            return false;

        if (occupiedSquares.ContainsKey(gridPosition))
            return false;

        occupiedSquares[gridPosition] = occupant;
        return true;
    }

    public void Release(Vector2Int gridPosition)
    {
        occupiedSquares.Remove(gridPosition);
    }

    public bool TryMove(Vector2Int fromPosition, Vector2Int toPosition, GameObject occupant)
    {
        if (!IsWithinBounds(toPosition))
            return false;

        if (occupiedSquares.ContainsKey(toPosition))
            return false;

        Release(fromPosition);
        occupiedSquares[toPosition] = occupant;
        return true;
    }
    public bool IsWithinBounds(Vector2Int gridPosition)
    {
        return gridPosition.x >= 0 && gridPosition.x < GRID_SIZE &&
               gridPosition.y >= 0 && gridPosition.y < GRID_SIZE;
    }

    public GameObject GetOccupant(Vector2Int gridPosition)
    {
        occupiedSquares.TryGetValue(gridPosition, out var occupant);
        return occupant;
    }

    public Vector2Int WorldToGrid(Vector3 worldPosition, float gridCellSize)
    {
        return new Vector2Int(
            Mathf.RoundToInt(worldPosition.x / gridCellSize),
            Mathf.RoundToInt(worldPosition.y / gridCellSize)
        );
    }

    public Vector3 GridToWorld(Vector2Int gridPosition, float gridCellSize)
    {
        return new Vector3(gridPosition.x * gridCellSize, gridPosition.y * gridCellSize, 0);
    }

    public Vector3 GetGridCellCenter(Vector2Int gridPosition, float gridCellSize)
    {
        return GridToWorld(gridPosition, gridCellSize) + new Vector3(gridCellSize * 0.5f, gridCellSize * 0.5f, 0);
    }

    public Vector2Int GetWinningSquare(Frog.ControlScheme controlScheme)
    {
        int index = controlScheme == Frog.ControlScheme.WASD ? 1 : 0;
        if (winningSquares == null || winningSquares.Length <= index)
            return new Vector2Int(4, 8);

        return winningSquares[index];
    }

    public bool IsWinningSquare(Vector2Int gridPosition)
    {
        if (winningSquares == null)
            return false;

        for (int i = 0; i < winningSquares.Length; i++)
        {
            if (winningSquares[i] == gridPosition)
                return true;
        }

        return false;
    }

    public bool IsWinningSquareForScheme(Vector2Int gridPosition, Frog.ControlScheme controlScheme)
    {
        return gridPosition == GetWinningSquare(controlScheme);
    }

    public bool CanMoveToPosition(Vector2Int gridPosition, Frog.ControlScheme controlScheme)
    {
        if (IsWinningSquareForScheme(gridPosition, controlScheme))
            return true;

        if (IsWinningSquare(gridPosition))
            return false;

        if (!IsWithinBounds(gridPosition))
            return false;

        if (occupiedSquares.ContainsKey(gridPosition))
            return false;

        return true;
    }
}