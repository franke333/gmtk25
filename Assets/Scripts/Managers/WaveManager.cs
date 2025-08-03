using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : SingletonClass<WaveManager>
{
    [SerializeField] private List<GameObject> enemyPrefabs;
    float cd = 0;

    public AnimationCurve spawnCD;
    public float duration = 4.25f*60;

    private void Update()
    {
        if (!GameManager.Instance.gameRunning)
            return;
        if(GameManager.Instance.gameTime > duration)
        {
            return;
        }
        cd -= Time.deltaTime;
        if(cd <= 0f)
        {
            cd = spawnCD.Evaluate(GameManager.Instance.gameTime / duration);
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
