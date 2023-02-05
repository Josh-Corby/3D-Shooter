using UnityEngine;
using System;

public class WeaponPickup : GameBehaviour
{
    public static event Action<GameObject> OnWeaponPickupTriggerEnter = null;
    public static event Action OnWeaponPickupTriggerExit = null;
    public static event Action<GameObject> OnWeaponPickup = null;

    [SerializeField] private GameObject weaponToPickUp;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == PM.player)
        {
            Debug.Log("Player collision");
            OnWeaponPickupTriggerEnter(weaponToPickUp);
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
        OnWeaponPickup(weaponToPickUp);
        OnWeaponPickupTriggerExit?.Invoke();
        Destroy(gameObject);
    }
}
