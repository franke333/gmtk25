using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : SingletonClass<WaveManager>
{
    [SerializeField] private List<GameObject> enemyPrefabs;
    float cd = 0;

    private void Update()
    {
        if (!GameManager.Instance.gameRunning)
            return;
        cd -= Time.deltaTime;
        if(cd <= 0f)
        {
            cd = 0.25f; // Reset cooldown to 1 second
            SpawnEnemy();
        }
    }


    void SpawnEnemy()
    {
        if (enemyPrefabs.Count == 0)
        {
            Debug.LogWarning("No enemy prefabs available to spawn.");
            return;
        }

        int randomIndex = Random.Range(0, enemyPrefabs.Count);
        GameObject enemyPrefab = enemyPrefabs[randomIndex];

        if (enemyPrefab != null)
        {
            GameObject enemyInstance = Instantiate(enemyPrefab, new Vector3(500,500,0), Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Selected enemy prefab is null.");
        }
    }
}
