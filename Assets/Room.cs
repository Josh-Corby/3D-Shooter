using UnityEngine;
using System;

public class Room : GameBehaviour
{
    public static event Action<Room> OnCombatRoomEntered = null;

    [SerializeField] private Door[] doors;
    [HideInInspector] public EnemySpawnController spawnController;
    private bool combatCleared = false;

    private void Awake()
    {
        spawnController = GetComponentInChildren<EnemySpawnController>();
    }

    private void OnEnable()
    {
        WaveManager.OnCombatStart += StartCombat;
        WaveManager.OnCombatEnd += OpenDoors;
    }

    private void OnDisable()
    {
        WaveManager.OnCombatStart -= StartCombat;
        WaveManager.OnCombatEnd -= OpenDoors;
    }

    private void Start()
    {
        CloseDoors();
    }

    private void StartCombat()
    {
        CloseDoors();
        spawnController.SpawnWave();
    }
    public void CheckCombatCleared()
    {
        if (combatCleared == true) return;

        OnCombatRoomEntered(this);
    }

    private void CloseDoors()
    {
        //if (this !=WM.currentRoom) return;

        foreach (Door door in doors)
        {
            door.Close();
        }

        //OnCombatRoomEntered(this);
    }

    public void OpenDoors()
    {
        if (this != WM.currentRoom) return;

        foreach (Door door in doors)
        {
            door.Open();
        }
    }
}
