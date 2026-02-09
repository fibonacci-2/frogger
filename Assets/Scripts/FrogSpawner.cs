using UnityEngine;

public class FrogSpawner : MonoBehaviour
{
    public GameObject[] carPrefabs;
    public int spawnGridX = 0;
    public int spawnGridY = 0;
    public SpawnDirection direction = SpawnDirection.Right;
    public int maxCarsAtOnce = 6;
    public int carSpeed = 1; // Number of grid squares the car moves per tick
    
    private float gridSize;

    void Start()
    {
        // Get grid size from GridManager
        if (GridManager.Instance != null)
            gridSize = GridManager.Instance.gridCellSize;
        
        // Subscribe to tick manager in Start, not Awake (ensures TickManager is initialized)
        if (TickManager.Instance != null)
            TickManager.Instance.OnTick += HandleTick;
        
        // Reset state on game start
        if (TickManager.Instance != null)
            TickManager.Instance.ResetTicks();
        Debug.Log("FrogSpawner started, ready to spawn cars");
    }

    void OnDisable()
    {
        if (TickManager.Instance != null)
            TickManager.Instance.OnTick -= HandleTick;
    }

    void HandleTick()
    {
        Debug.Log("Handling tick in FrogSpawner");
        int currentCarCount = FindObjectsOfType<Car>().Length;
        if (currentCarCount < maxCarsAtOnce)
        {
            Debug.Log($"Spawning car. CurrentCars: {currentCarCount}, MaxCars: {maxCarsAtOnce}");
            SpawnCar();
        }
    }

    void SpawnCar()
    {
        if (carPrefabs.Length == 0) return;
        
        // Spawn at grid coordinates converted to world position
        Vector3 worldPos = new Vector3(spawnGridX * gridSize, spawnGridY * gridSize, 0);
        GameObject prefab = carPrefabs[Random.Range(0, carPrefabs.Length)];
        GameObject car = Instantiate(prefab, worldPos, Quaternion.identity);
        
        Car carScript = car.GetComponent<Car>();
        if (carScript != null)
        {
            carScript.SetGridPosition(spawnGridX, spawnGridY);
            // random speed
            carScript.SetSpeed(Random.Range(1, 4));
            carScript.direction = (int)direction;
        }
    }


    public void Reset()
    {
        Debug.Log("FrogSpawner reset");
    }
}

public enum SpawnDirection
{
    Left = -1,
    Right = 1
}
