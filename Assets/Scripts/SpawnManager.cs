using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : GameBehaviour<SpawnManager>
{
    public Transform spawnCentre;
    public Vector3 spawnSize;

    public int enemiesToSpawn;

    public List<GameObject> enemiesAlive = new List<GameObject>();

    public GameObject enemyPrefab;

    public bool spawnEnemies;

    private void Start()
    {
        if (spawnEnemies)
        {

        SpawnWave();
        }
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
        float x = Random.Range(-spawnSize.x / 2, spawnSize.x / 2);
        float y = Random.Range(-spawnSize.y / 2, spawnSize.y / 2);
        float z = Random.Range(-spawnSize.z / 2, spawnSize.z / 2);
        Vector3 spawnPosition = new Vector3(spawnCentre.position.x + x, spawnCentre.position.y + y, spawnCentre.position.z + z);
        return spawnPosition;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(spawnCentre.position, spawnSize);
    }
}
