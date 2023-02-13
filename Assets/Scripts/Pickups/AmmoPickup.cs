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
            if (PWM.currentWeapon == null) return;

            OnAmmoPickup(ammoAmount);
            PWM.currentWeapon.AddAmmo(ammoAmount);

            Destroy(gameObject);
        }
    }
}
