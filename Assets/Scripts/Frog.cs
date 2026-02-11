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
    private int startGridX;
    private int startGridY;

    void Start()
    {
        if (GridManager.Instance != null)
            gridSize = GridManager.Instance.gridCellSize;
        
        UpdateFrogGridPosition();
        startGridX = frogGridX;
        startGridY = frogGridY;
        CenterFrogInCell();
    }

    void Update()
    {
        // if (!isPlayerControlled)
        //     return;

        GetMovementKeys(out KeyCode upKey, out KeyCode downKey, out KeyCode leftKey, out KeyCode rightKey);

        if (Input.GetKeyDown(upKey))
        {
            if (CanMoveUp() && TryMove(0, 1))
            {
                TickManager.Instance?.AdvanceTick();
                GameManager.Instance?.CheckWinCondition();
                CheckAllFrogsCollision();
            }
        }
        if (Input.GetKeyDown(downKey))
        {
            if (TryMove(0, -1))
            {
                TickManager.Instance?.AdvanceTick();
                GameManager.Instance?.CheckWinCondition();
                CheckAllFrogsCollision();
            }
        }
        if (Input.GetKeyDown(leftKey))
        {
            if (TryMove(-1, 0))
            {
                TickManager.Instance?.AdvanceTick();
                GameManager.Instance?.CheckWinCondition();
                CheckAllFrogsCollision();
            }
        }
        if (Input.GetKeyDown(rightKey))
        {
            if (TryMove(1, 0))
            {
                TickManager.Instance?.AdvanceTick();
                GameManager.Instance?.CheckWinCondition();
                CheckAllFrogsCollision();
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
        frogGridX = Mathf.RoundToInt((transform.position.x - gridSize * 0.5f) / gridSize);
        frogGridY = Mathf.RoundToInt((transform.position.y - gridSize * 0.5f) / gridSize);
    }

    void CenterFrogInCell()
    {
        transform.position = new Vector3(frogGridX * gridSize + gridSize * 0.5f, frogGridY * gridSize + gridSize * 0.5f, 0);
    }

    void CheckCollisionWithCars()
    {
        UpdateFrogGridPosition();
        
        if (GridManager.Instance != null)
        {
            Vector2Int frogCell = new Vector2Int(frogGridX, frogGridY);
            GameObject occupant = GridManager.Instance.GetOccupant(frogCell);
            
            if (occupant != null && occupant.CompareTag("car"))
                GameManager.Instance.Pause();
        }
    }

    void CheckAllFrogsCollision()
    {
        Frog[] frogs = FindObjectsOfType<Frog>();
        for (int i = 0; i < frogs.Length; i++)
        {
            frogs[i].CheckCollisionWithCars();
        }
    }

    public bool IsOnWinningSquare()
    {
        UpdateFrogGridPosition();
        Vector2Int frogCell = new Vector2Int(frogGridX, frogGridY);
        Vector2Int winSquare = GridManager.Instance.GetWinningSquare(controlScheme);
        return frogCell == winSquare;
    }

    bool TryMove(int deltaX, int deltaY)
    {
        int newGridX = frogGridX + deltaX;
        int newGridY = frogGridY + deltaY;
        Vector2Int newPosition = new Vector2Int(newGridX, newGridY);

        if (!GridManager.Instance.CanMoveToPosition(newPosition, controlScheme))
        {
            return false;
        }

        if (IsOtherFrogBlocking(newPosition))
        {
            return false;
        }

        frogGridX = newGridX;
        frogGridY = newGridY;
        CenterFrogInCell();
        return true;
    }

    bool IsOtherFrogBlocking(Vector2Int targetPosition)
    {
        Frog[] frogs = FindObjectsOfType<Frog>();
        for (int i = 0; i < frogs.Length; i++)
        {
            if (frogs[i] == this)
                continue;

            Vector2Int otherPosition = frogs[i].GetGridPosition();
            if (otherPosition == targetPosition)
                return true;
        }

        return false;
    }

    bool CanMoveUp()
    {
        Frog[] frogs = FindObjectsOfType<Frog>();
        for (int i = 0; i < frogs.Length; i++)
        {
            if (frogs[i] == this)
                continue;

            Vector2Int otherPosition = frogs[i].GetGridPosition();
            int yDistance = frogGridY - otherPosition.y;
            if (yDistance <= 2)
                return true;
        }

        return false;
    }

    public Vector2Int GetGridPosition()
    {
        return new Vector2Int(frogGridX, frogGridY);
    }

    public void ResetToStartPosition()
    {
        frogGridX = startGridX;
        frogGridY = startGridY;
        CenterFrogInCell();
    }
}
