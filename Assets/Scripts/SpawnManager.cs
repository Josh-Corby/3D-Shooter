using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : GameBehaviour<SpawnManager>
{
    public Transform spawnCentre;
    public Vector3 spawnSize;
    public int enemiesToSpawn;
    public List<GameObject> enemiesAlive = new List<GameObject>();
    public GameObject[] enemyPrefabs;
    public bool spawnEnemies;

    [SerializeField] private LayerMask groundMask;

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
            GameObject randomEnemyType = GetRandomEnemyType(); 
            GameObject enemy = Instantiate(randomEnemyType, GetRandomSpawnPosition(randomEnemyType.GetComponent<EnemyBase>()), Quaternion.identity);
            enemiesAlive.Add(enemy);
        }
    }

    private GameObject GetRandomEnemyType()
    {
        int randomEnemyTypeIndex = Random.Range(0, enemyPrefabs.Length);
        GameObject randomEnemyType = enemyPrefabs[randomEnemyTypeIndex];
        return randomEnemyType;
    }

    private Vector3 GetRandomSpawnPosition(EnemyBase enemy)
    {
        float x = Random.Range(-spawnSize.x / 2, spawnSize.x / 2);
        float y = Random.Range(-spawnSize.y / 2, spawnSize.y / 2);
        float z = Random.Range(-spawnSize.z / 2, spawnSize.z / 2);
        Vector3 spawnPosition = new Vector3(spawnCentre.position.x + x, spawnCentre.position.y + y, spawnCentre.position.z + z);

        if (enemy.flying == false)
        {
            Ray ray = new Ray(spawnPosition, Vector3.down);
            if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundMask))
            {
                spawnPosition = hit.point + new Vector3(0, 1, 0);
            }
        }
        return spawnPosition;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(spawnCentre.position, spawnSize);
    }
}
