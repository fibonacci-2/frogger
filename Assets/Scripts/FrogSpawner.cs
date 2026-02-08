using UnityEngine;

public class FrogSpawner : MonoBehaviour
{
    public GameObject[] carPrefabs;
    public int spawnTickInterval = 4;
    public int spawnGridX = 0;
    public int spawnGridY = 0;
    public SpawnDirection direction = SpawnDirection.Right;
    public float gridSize = 16f;
    
    private int lastSpawnTick = 0;

    void Awake()
    {
        if (TickManager.Instance != null)
            TickManager.Instance.OnTick += HandleTick;
    }

    void OnDisable()
    {
        if (TickManager.Instance != null)
            TickManager.Instance.OnTick -= HandleTick;
    }

    void HandleTick()
    {
        int currentTick = TickManager.Instance.tickCounter;
        if (currentTick - lastSpawnTick >= spawnTickInterval)
        {
            SpawnCar();
            lastSpawnTick = currentTick;
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
            carScript.ticksPerMove = Random.Range(1, 4);
            carScript.direction = (int)direction;
        }
    }
}

public enum SpawnDirection
{
    Left = -1,
    Right = 1
}
