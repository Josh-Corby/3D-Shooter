using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : GameBehaviour<SpawnManager>
{
    public Transform spawnCentre;
    public float spawnSize;

    public int enemiesToSpawn;

    public List<GameObject> enemiesAlive = new List<GameObject>();

    public GameObject enemyPrefab;

    private void Start()
    {
        SpawnWave();
    }

    private void SpawnWave()
    {
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab, GetRandomSpawnPosition(), Quaternion.identity);
            enemiesAlive.Add(enemy);
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float x = Random.Range(-spawnSize, spawnSize);
        float y = Random.Range(-spawnSize, spawnSize);
        Vector3 spawnPosition = new Vector3(spawnCentre.position.x + x, 10, spawnCentre.position.z + y);
        return spawnPosition;
    }
}
