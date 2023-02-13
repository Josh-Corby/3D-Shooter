using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemySpawnController : GameBehaviour
{
    [SerializeField] private Transform spawnCentre;
    [SerializeField] private Vector3 spawnSize;
    [SerializeField] private int enemiesToSpawn;
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private LayerMask groundMask;

    private float spawnBufferTime = 0.5f;
    private int spawnBufferSize = 20;

    public void StartSpawningWave()
    {
        StartCoroutine(SpawnWave());
    }

    private IEnumerator SpawnWave()
    {
        yield return new WaitForSeconds(spawnBufferTime);
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            GameObject randomEnemyType = GetRandomEnemyType(); 
            GameObject enemy = Instantiate(randomEnemyType, GetRandomSpawnPosition(randomEnemyType.GetComponent<EnemyBase>()), Quaternion.identity);
            WM.enemiesAlive.Add(enemy);
            yield return new WaitForSeconds(spawnBufferTime);
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
            //if not flying then it is a navmesh unit
            //find the closest navmesh point to random point found

            //Ray ray = new Ray(spawnPosition, Vector3.down);
            //if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundMask))
            //{
            //    spawnPosition = hit.point + new Vector3(0, 1, 0);
            //    return spawnPosition;
            //}

            //check if random point is on navmesh
            if(NavMesh.SamplePosition(spawnPosition,out NavMeshHit navHit, 0, groundMask))
            {
                return navHit.position;
            }

            //else find a new random position
            else
            {
                NavMesh.SamplePosition(spawnPosition, out navHit, spawnBufferSize, groundMask);
                return navHit.position;
            }
        }
        return spawnPosition;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(spawnCentre.position, spawnSize);
    }
}
