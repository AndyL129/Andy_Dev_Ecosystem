using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrainSpawner : MonoBehaviour
{
    public GameObject brainPrefab; // Assign the Brain prefab in the inspector
    public float minSpawnTime = 2f; // Minimum time between spawns
    public float maxSpawnTime = 5f; // Maximum time between spawns

    private void Start()
    {
        StartCoroutine(SpawnBrainCoroutine()); // Start the spawning coroutine
    }

    private IEnumerator SpawnBrainCoroutine()
    {
        while (true)
        {
            // Wait for a random time between minSpawnTime and maxSpawnTime
            float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(waitTime);

            // Generate a random position within screen bounds
            Vector3 spawnPosition = GetRandomPosition();
            Instantiate(brainPrefab, spawnPosition, Quaternion.identity);
        }
    }

    private Vector3 GetRandomPosition()
    {
        // Calculate random position within screen boundaries
        float screenX = Random.Range(-8f, 8f);
        float screenY = Random.Range(-4f, 4f);
        return new Vector3(screenX, screenY, 0f);
    }
}
