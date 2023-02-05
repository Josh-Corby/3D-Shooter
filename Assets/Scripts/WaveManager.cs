using System.Collections.Generic;
using UnityEngine;
using System;

public class WaveManager : GameBehaviour<WaveManager>
{
    public static event Action OnCombatStart;
    public static event Action OnCombatEnd;

    public List<GameObject> enemiesAlive = new List<GameObject>();
    public Room currentRoom;
    public EnemySpawnController currentSpawner;


    private void OnEnable()
    {
        Room.OnCombatRoomEntered += SetRoom;
        EnemyBase.OnEnemyDied += RemoveDeadEnemy;
    }

    private void OnDisable()
    {
        Room.OnCombatRoomEntered -= SetRoom;
        EnemyBase.OnEnemyDied -= RemoveDeadEnemy;
    }

    public void SetRoom(Room room)
    {
        currentRoom = room;
        currentSpawner = room.spawnController;
        StartCombat();
    }

    public bool CheckCurrentRoom(Room room)
    {
        return room == currentRoom;
    }

    private void StartCombat()
    {
        OnCombatStart?.Invoke();
    }

    private void EndCombat()
    {
        OnCombatEnd?.Invoke();
    }

    private void RemoveDeadEnemy(EnemyBase deadEnemy)
    {
        if (enemiesAlive.Contains(deadEnemy.gameObject))
        {
            enemiesAlive.Remove(deadEnemy.gameObject);
        }

        CheckEnemiesAlive();
    }
    private void CheckEnemiesAlive()
    {
        if(enemiesAlive.Count == 0)
        {
            EndCombat();
        }
    }
}
