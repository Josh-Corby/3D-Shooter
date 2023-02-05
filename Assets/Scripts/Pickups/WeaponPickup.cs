using UnityEngine;
using System;

public class WeaponPickup : GameBehaviour
{
    public static event Action OnWeaponPickupTriggerEnter = null;
    public static event Action OnWeaponPickupTriggerExit = null;

    [SerializeField] private GameObject weaponToPickUp;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == PM.player)
        {
            OnWeaponPickupTriggerEnter?.Invoke();
            InputManager.OnInteract += PickupWeapon;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == PM.player)
        {
            OnWeaponPickupTriggerExit?.Invoke();
            InputManager.OnInteract -= PickupWeapon;
        }
    }

    private void PickupWeapon()
    {

    }
}
