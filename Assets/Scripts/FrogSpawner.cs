using UnityEngine;

public class FrogSpawner : MonoBehaviour
{
    public GameObject[] carPrefabs;
    public int spawnTickInterval = 4;
    public int spawnGridX = 0;
    public int spawnGridY = 0;
    public SpawnDirection direction = SpawnDirection.Right;
    public float gridSize = 16f;
    public int maxCarsAtOnce = 6;
    
    private int lastSpawnTick = 0;
    private int activeCarCount = 0;

    void Awake()
    {
        if (TickManager.Instance != null)
            TickManager.Instance.OnTick += HandleTick;
    }

    void Start()
    {
        // Reset state on game start
        activeCarCount = 0;
        lastSpawnTick = 0;
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
        int currentTick = TickManager.Instance.tickCounter;
        if (currentTick - lastSpawnTick >= spawnTickInterval && activeCarCount < maxCarsAtOnce)
        {
            Debug.Log($"Spawning car. Tick: {currentTick}, LastSpawnTick: {lastSpawnTick}, ActiveCars: {activeCarCount}");
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

    public void IncrementCarCount()
    {
        activeCarCount++;
    }

    public void DecrementCarCount()
    {
        activeCarCount--;
    }


    public void Reset()
    {
        activeCarCount = 0;
        lastSpawnTick = 0;
        Debug.Log("FrogSpawner reset");
    }
}

public enum SpawnDirection
{
    Left = -1,
    Right = 1
}
