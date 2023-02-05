using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerWeaponManager : GameBehaviour<PlayerWeaponManager>
{
    public static event Action<GunBase> OnWeaponChange = null;

    [SerializeField] private GameObject currentWeaponObject;
    [HideInInspector] public GunBase currentWeapon;
    public List<GameObject> playerWeapons = new List<GameObject>();
    private int currentWeaponIndex;

    private void OnEnable()
    {
        InputManager.OnScroll += ChangeWeapons;
        AmmoPickup.OnAmmoPickup += RestoreAmmo;
        WeaponPickup.OnWeaponPickup += AddWeapon;
    }
    private void OnDisable()
    {
        InputManager.OnScroll -= ChangeWeapons;
        AmmoPickup.OnAmmoPickup -= RestoreAmmo;
        WeaponPickup.OnWeaponPickup -= AddWeapon;
    }

    private void Start()
    {
        InitializeWeapons();
    }

    private void InitializeWeapons()
    {
        if (playerWeapons.Count <= 0) return;

        currentWeaponIndex = 0;
        for (int i = 0; i < playerWeapons.Count; i++)
        {
            playerWeapons[i].SetActive(false);
        }
        currentWeaponObject = playerWeapons[0];
        currentWeapon = currentWeaponObject.GetComponent<GunBase>();
        currentWeaponObject.SetActive(true);

        OnWeaponChange(currentWeapon);
    }

    private void ChangeWeapons(float _input)
    {
        if(playerWeapons.Count < 2)
        {
            return;
        }

        if (_input > 0)
        {
            currentWeaponIndex -= 1;

            if (currentWeaponIndex < 0)
            {
                currentWeaponIndex = playerWeapons.Count - 1;
            }
        }

        if (_input < 0)
        {
            currentWeaponIndex += 1;

            if (currentWeaponIndex > playerWeapons.Count - 1)
            {
                currentWeaponIndex = 0;
            }
        }

        currentWeaponObject.SetActive(false);
        currentWeaponObject = playerWeapons[currentWeaponIndex];
        currentWeaponObject.SetActive(true);
        currentWeapon = currentWeaponObject.GetComponent<GunBase>();
        OnWeaponChange(currentWeapon);
    }

    private void EquipWeapon(GameObject weapon)
    {
        currentWeaponObject = weapon;
        currentWeapon = weapon.GetComponent<GunBase>();
        OnWeaponChange(currentWeapon);
    }

    private void RestoreAmmo(int ammoAmount)
    {
        currentWeapon.AddAmmo(ammoAmount);
    }

    public bool CheckIfCurrentWeapon(GunBase gun)
    {
        bool isCurrentWeapon = gun == currentWeapon;
        return isCurrentWeapon;
    }

    private void AddWeapon(GameObject weaponToAdd)
    {
        if (playerWeapons.Contains(weaponToAdd))
        {
            return;
        }
        playerWeapons.Add(weaponToAdd);
        GameObject gun = Instantiate(weaponToAdd, PWM.transform);

        if(playerWeapons.Count == 1)
        {
            EquipWeapon(weaponToAdd);
        }

        else
        {
            gun.SetActive(false);
        }
    }
}
