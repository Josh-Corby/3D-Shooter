using UnityEngine;
using System;

public class Room : GameBehaviour
{
    public static event Action<Room> OnCombatRoomEntered = null;

    [SerializeField] private Door[] doors;
    [HideInInspector] public EnemySpawnController spawnController;
    [SerializeField] private bool combatCleared = false;

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
        if (!WM.CheckCurrentRoom(this)) return;
        CloseDoors();
        spawnController.StartSpawningWave();
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
        foreach (Door door in doors)
        {
            door.Close();
        }
    }

    public void OpenDoors()
    {
        foreach (Door door in doors)
        {
            door.Open();
        }
    }

    private void DisableCombatTriggers()
    {
        foreach (Door door in doors)
        {
            door.trigger.gameObject.SetActive(false);
        }
    }

    private void EnableCombatTriggers()
    {
        foreach (Door door in doors)
        {
            door.trigger.gameObject.SetActive(true);
        }
    }
}
