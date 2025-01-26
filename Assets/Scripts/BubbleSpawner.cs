using System.Collections;
using UnityEngine;

public class BubbleSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [Tooltip("The prefab to spawn.")]
    public GameObject bubblePrefab;

    [Tooltip("Minimum spawn interval in seconds.")]
    public float minSpawnInterval = 8f; // Minimum amount of time between spawns

    [Tooltip("Maximum spawn interval in seconds.")]
    public float maxSpawnInterval = 16f; // Maximum amount of time between spawns

    [Tooltip("Spawn range on the X axis.")]
    public float rangeX = 10f;

    [Tooltip("Spawn range on the Z axis.")]
    public float rangeZ = 10f;

    [Tooltip("Spawn range on the Y axis. (Optional)")]
    public float rangeY = 0f;

    [Tooltip("Check to continuously spawn objects.")]
    public bool continuousSpawn = true;

    private void Start()
    {
        if (continuousSpawn)
        {
            StartCoroutine(SpawnWithVariation());
        }
    }

    private IEnumerator SpawnWithVariation()
    {
        while (true) // Infinite loop if continuous spawn is enabled
        {
            // Spawn the object
            SpawnObject();

            // Randomize the spawn interval within the min-max range
            float randomInterval = Random.Range(minSpawnInterval, maxSpawnInterval);

            // Wait for the random interval before continuing the loop
            yield return new WaitForSeconds(randomInterval);
        }
    }
    
    public void SpawnObject()
    {
        if (bubblePrefab == null)
        {
            Debug.LogError("No object assigned to spawn!");
            return;
        }

        // Generate a random position within the defined range
        Vector3 randomPosition = new Vector3(
            Random.Range(-rangeX, rangeX), 
            Random.Range(-rangeY, rangeY), 
            Random.Range(-rangeZ, rangeZ)
        );

        // Spawn the object at the random position relative to the spawner
        Instantiate(bubblePrefab, randomPosition + transform.position, Quaternion.identity);
    }

    // Optional: For testing without continuous spawning
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            SpawnObject();
        }
    }
}
