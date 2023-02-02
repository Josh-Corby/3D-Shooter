using UnityEngine;
using System;

public class AmmoPickup : GameBehaviour
{
    public static event Action<int> OnAmmoPickup = null;
    [SerializeField] private int ammoAmount;

    private void OnTriggerEnter(Collider other)
    {
        ProcessCollision(other.gameObject);
    }

    private void ProcessCollision(GameObject collider)
    {
        if(collider == PM.gameObject)
        {
            OnAmmoPickup(ammoAmount);
            PM.currentWeapon.AddAmmo(ammoAmount);

            Destroy(gameObject);
        }
    }
}
