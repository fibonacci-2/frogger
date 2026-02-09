using UnityEngine;

public class FrogSpawner : MonoBehaviour
{
    public GameObject[] carPrefabs;
    public int spawnGridX = 0;
    public int spawnGridY = 0;
    public SpawnDirection direction = SpawnDirection.Right;
    public int maxCarsAtOnce = 6;
    
    private float gridSize;

    void Start()
    {
        // Get grid size from GridManager
        if (GridManager.Instance != null)
            gridSize = GridManager.Instance.gridCellSize;
        
        Debug.Log($"[Spawner] Started at grid ({spawnGridX}, {spawnGridY}), direction={direction}, maxCars={maxCarsAtOnce}");
    }

    void OnDisable()
    {
        if (TickManager.Instance != null)
            TickManager.Instance.OnTick -= HandleTick;
    }

    void HandleTick()
    {
        int currentCarCount = FindObjectsOfType<Car>().Length;
        if (currentCarCount < maxCarsAtOnce)
        {
            SpawnCar();
        }
    }

    void SpawnCar()
    {
        if (carPrefabs.Length == 0) return;
        
        // Spawn at grid coordinates converted to world position (centered in cell)
        Vector3 worldPos = new Vector3(spawnGridX * gridSize + gridSize * 0.5f, spawnGridY * gridSize + gridSize * 0.5f, 0);
        GameObject prefab = carPrefabs[Random.Range(0, carPrefabs.Length)];
        GameObject car = Instantiate(prefab, worldPos, Quaternion.identity);
        
        Car carScript = car.GetComponent<Car>();
        if (carScript != null)
        {
            carScript.SetGridPosition(spawnGridX, spawnGridY);
            carScript.SetSpeed(Random.Range(1, 4));
            carScript.direction = (int)direction;
        }
    }


    public void Reset()
    {
        // Debug.Log("FrogSpawner reset");
    }
}

public enum SpawnDirection
{
    Left = -1,
    Right = 1
}
