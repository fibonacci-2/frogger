using UnityEngine;

public class FrogSpawner : MonoBehaviour
{
    public GameObject[] carPrefabs;
    // Optional frog prefab to spawn once at start
    public GameObject frogPrefab;
    public bool spawnInitialFrog = true;
    public float spawnInterval = 2f;
    public SpawnDirection direction = SpawnDirection.Right;
    private float spawnTimer = 0f;

    void Start()
    {
        if (spawnInitialFrog && frogPrefab != null)
        {
            Instantiate(frogPrefab, transform.position, Quaternion.identity);
        }
    }

    void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            SpawnCar();
            float randomVariation = Random.Range(-2f, 2f);
            spawnTimer = 0f;
            spawnInterval += randomVariation;
        }
    }

    void SpawnCar()
    {
        if (carPrefabs.Length == 0) return;
        GameObject prefab = carPrefabs[Random.Range(0, carPrefabs.Length)];
        GameObject car = Instantiate(prefab, transform.position, Quaternion.identity);
        Car carScript = car.GetComponent<Car>();
        if (carScript != null)
            carScript.direction = (int)direction;
    }
}

public enum SpawnDirection
{
    Left = -1,
    Right = 1
}
