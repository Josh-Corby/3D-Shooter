using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerWeaponManager : GameBehaviour<PlayerWeaponManager>
{
    public static event Action<GunBase> OnWeaponChange = null;

    [SerializeField] private GameObject currentWeaponObject;
    [HideInInspector] public GunBase currentWeapon;
    public GameObject[] playerWeapons;
    private int currentWeaponIndex;

    private void OnEnable()
    {
        InputManager.OnScroll += ChangeWeapons;
        AmmoPickup.OnAmmoPickup += RestoreAmmo;
    }
    private void OnDisable()
    {
        InputManager.OnScroll -= ChangeWeapons;
    }

    private void Start()
    {
        InitializeWeapons();
    }

    private void InitializeWeapons()
    {
        currentWeaponIndex = 0;
        for (int i = 0; i < playerWeapons.Length; i++)
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
        if (_input > 0)
        {
            currentWeaponIndex -= 1;

            if (currentWeaponIndex < 0)
            {
                currentWeaponIndex = playerWeapons.Length - 1;
            }
        }

        if (_input < 0)
        {
            currentWeaponIndex += 1;

            if (currentWeaponIndex > playerWeapons.Length - 1)
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
    private void RestoreAmmo(int ammoAmount)
    {
        currentWeapon.AddAmmo(ammoAmount);
    }

    public bool CheckIfCurrentWeapon(GunBase gun)
    {
        bool isCurrentWeapon = gun == currentWeapon;
        return isCurrentWeapon;
    }
}
