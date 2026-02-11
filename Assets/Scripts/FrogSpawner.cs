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
        if (GridManager.Instance != null)
            gridSize = GridManager.Instance.gridCellSize;
        
        if (TickManager.Instance != null)
            TickManager.Instance.OnTick += HandleTick;
        
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
            if (Random.Range(0, 2) == 0)
                SpawnCar();
        }
    }

    void SpawnCar()
    {
        if (carPrefabs.Length == 0) return;
        
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
    }
}

public enum SpawnDirection
{
    Left = -1,
    Right = 1
}
