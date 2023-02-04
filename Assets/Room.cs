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
        WaveManager.OnCombatEnd += EndCombat;
    }

    private void OnDisable()
    {
        WaveManager.OnCombatStart -= StartCombat;
        WaveManager.OnCombatEnd -= EndCombat;
    }

    private void Start()
    {
        EnableCombatTriggers();
        combatCleared = false;
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

    public void EndCombat()
    {
        OpenDoors();
        DisableCombatTriggers();
    }

    private void CloseDoors()
    {
        if (this !=WM.currentRoom) return;

        foreach (Door door in doors)
        {
            door.Close();
        }
    }

    public void OpenDoors()
    {
        if (this != WM.currentRoom) return;

        foreach (Door door in doors)
        {
            door.Open();
        }
    }

    private void DisableCombatTriggers()
    {
        foreach (Door door in doors)
        {
            door.trigger.enabled = false;
        }
    }

    private void EnableCombatTriggers()
    {
        foreach (Door door in doors)
        {
            door.trigger.enabled = true;
        }
    }
}
