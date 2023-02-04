using UnityEngine;

public class EnemySpawnController : GameBehaviour
{
    [SerializeField] private Transform spawnCentre;
    [SerializeField] private Vector3 spawnSize;
    [SerializeField] private int enemiesToSpawn;
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private LayerMask groundMask;

    public void SpawnWave()
    {
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            GameObject randomEnemyType = GetRandomEnemyType(); 
            GameObject enemy = Instantiate(randomEnemyType, GetRandomSpawnPosition(randomEnemyType.GetComponent<EnemyBase>()), Quaternion.identity);
            WM.enemiesAlive.Add(enemy);
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(spawnCentre.position, spawnSize);
    }
}
