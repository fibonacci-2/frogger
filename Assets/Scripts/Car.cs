using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Car moves grid squares when TickManager issues a tick.
// Speed is controlled exclusively by the spawner via SetSpeed().
public class Car : MonoBehaviour
{
    private int squaresPerMove = 1;
    public int direction = 1; // 1 = right, -1 = left

    private int gridX;
    private int gridY;
    private FrogSpawner spawner;
    private bool gridPositionSet = false;
    private float gridSize;

    // Visual / animation
    public Sprite[] moveSprites; // frames to cycle while moving
    public float moveDuration = 0.12f; // seconds per square visual move
    private SpriteRenderer spriteRenderer;
    private Queue<Vector3> moveTargets = new Queue<Vector3>();
    private bool isProcessingMoveQueue = false;
    private TextMeshProUGUI speedText;

    public void SetGridPosition(int x, int y)
    {
        gridX = x;
        gridY = y;
        gridPositionSet = true;
        gridSize = GridManager.Instance.gridCellSize;
        transform.position = new Vector3(x * gridSize + gridSize * 0.5f, y * gridSize + gridSize * 0.5f, 0);
    }

    public void SetSpeed(int speed)
    {
        squaresPerMove = speed;
    }

    void Start()
    {
        // Get grid size from GridManager
        if (GridManager.Instance != null)
            gridSize = GridManager.Instance.gridCellSize;
        
        // If grid position wasn't explicitly set, calculate from world position
        if (!gridPositionSet)
        {
            gridX = Mathf.RoundToInt(transform.position.x / gridSize);
            gridY = Mathf.RoundToInt(transform.position.y / gridSize);
        }
        
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
        
        // Setup visual components
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        
        // Setup text component for speed display
        speedText = GetComponent<TextMeshProUGUI>();
        if (speedText == null)
            speedText = GetComponentInChildren<TextMeshProUGUI>();
        
        // Display the current speed on the text
        if (speedText != null)
            speedText.text = squaresPerMove.ToString();
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
        // Move by squaresPerMove squares each tick
        for (int i = 0; i < squaresPerMove; i++)
        {
            TryMove();
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
            // Update logical position immediately
            gridX = newGridX;

            // Enqueue visual target (center of the grid cell). The grid occupancy already moved.
            Vector3 target = GridManager.Instance.GetGridCellCenter(new Vector2Int(gridX, gridY), gridSize);
            moveTargets.Enqueue(target);
            if (!isProcessingMoveQueue)
                StartCoroutine(ProcessMoveQueue());
        }
        // If occupied, just wait for next tick
    }

    IEnumerator ProcessMoveQueue()
    {
        isProcessingMoveQueue = true;
        while (moveTargets.Count > 0)
        {
            Vector3 start = transform.position;
            Vector3 end = moveTargets.Dequeue();

            // If there's no sprite renderer or moveSprites, just lerp without frame animation
            if (spriteRenderer == null || moveSprites == null || moveSprites.Length == 0)
            {
                float t = 0f;
                while (t < moveDuration)
                {
                    t += Time.deltaTime;
                    transform.position = Vector3.Lerp(start, end, Mathf.Clamp01(t / moveDuration));
                    yield return null;
                }
                transform.position = end;
            }
            else
            {
                // Cycle through sprites while moving
                int frameCount = Mathf.Max(1, moveSprites.Length);
                float frameTime = moveDuration / frameCount;
                float elapsed = 0f;
                int frame = -1;
                while (elapsed < moveDuration)
                {
                    elapsed += Time.deltaTime;
                    transform.position = Vector3.Lerp(start, end, Mathf.Clamp01(elapsed / moveDuration));

                    // update frame
                    int newFrame = Mathf.Min(frameCount - 1, (int)(elapsed / frameTime));
                    if (newFrame != frame)
                    {
                        frame = newFrame;
                        spriteRenderer.sprite = moveSprites[frame];
                    }

                    yield return null;
                }
                transform.position = end;
                // reset to first frame (idle) if available
                spriteRenderer.sprite = moveSprites[0];
            }
        }
        isProcessingMoveQueue = false;
    }
}


