using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Car : MonoBehaviour
{
    private int squaresPerMove = 1;
    public int direction = 1;

    private int gridX;
    private int gridY;
    private FrogSpawner spawner;
    private bool gridPositionSet = false;
    private float gridSize;

    public Sprite[] moveSprites;
    public float moveDuration = 0.12f;
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
        if (GridManager.Instance != null)
            gridSize = GridManager.Instance.gridCellSize;
        
        if (!gridPositionSet)
        {
            gridX = Mathf.RoundToInt(transform.position.x / gridSize);
            gridY = Mathf.RoundToInt(transform.position.y / gridSize);
        }
        
        Debug.Log($"Car spawned at grid ({gridX}, {gridY})");
        
        if (GridManager.Instance != null)
        {
            if (!GridManager.Instance.TryOccupy(new Vector2Int(gridX, gridY), gameObject))
            {
                Debug.LogWarning($"Failed to occupy grid ({gridX}, {gridY})");
                Destroy(gameObject);
            }
        }
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        
        speedText = GetComponent<TextMeshProUGUI>();
        if (speedText == null)
            speedText = GetComponentInChildren<TextMeshProUGUI>();
        
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
        
        if (GridManager.Instance != null)
            GridManager.Instance.Release(new Vector2Int(gridX, gridY));
    }

    void HandleTick()
    {
        for (int i = 0; i < squaresPerMove; i++)
        {
            TryMove();
        }
    }

    void TryMove()
    {
        int newGridX = gridX + direction;

        if (!GridManager.Instance.IsWithinBounds(new Vector2Int(newGridX, gridY)))
        {
            Debug.Log($"Car reached end of grid at ({gridX}, {gridY})");
            Destroy(gameObject);
            return;
        }

        if (GridManager.Instance.TryMove(new Vector2Int(gridX, gridY), new Vector2Int(newGridX, gridY), gameObject))
        {
            gridX = newGridX;

            Vector3 target = GridManager.Instance.GetGridCellCenter(new Vector2Int(gridX, gridY), gridSize);
            moveTargets.Enqueue(target);
            if (!isProcessingMoveQueue)
                StartCoroutine(ProcessMoveQueue());
            
            CheckCollisionWithFrogsAtPosition(new Vector2Int(gridX, gridY));
        }
    }

    void CheckCollisionWithFrogsAtPosition(Vector2Int carPosition)
    {
        Frog[] frogs = FindObjectsOfType<Frog>();
        for (int i = 0; i < frogs.Length; i++)
        {
            if (frogs[i].GetGridPosition() == carPosition)
            {
                GameManager.Instance.Pause();
                return;
            }
        }
    }

    IEnumerator ProcessMoveQueue()
    {
        isProcessingMoveQueue = true;
        while (moveTargets.Count > 0)
        {
            Vector3 start = transform.position;
            Vector3 end = moveTargets.Dequeue();

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
                int frameCount = Mathf.Max(1, moveSprites.Length);
                float frameTime = moveDuration / frameCount;
                float elapsed = 0f;
                int frame = -1;
                while (elapsed < moveDuration)
                {
                    elapsed += Time.deltaTime;
                    transform.position = Vector3.Lerp(start, end, Mathf.Clamp01(elapsed / moveDuration));

                    int newFrame = Mathf.Min(frameCount - 1, (int)(elapsed / frameTime));
                    if (newFrame != frame)
                    {
                        frame = newFrame;
                        spriteRenderer.sprite = moveSprites[frame];
                    }

                    yield return null;
                }
                transform.position = end;
                spriteRenderer.sprite = moveSprites[0];
            }
        }
        isProcessingMoveQueue = false;
    }
}


