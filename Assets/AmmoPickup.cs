using UnityEngine;
using System;

public class AmmoPickup : GameBehaviour
{
    [SerializeField] private int ammoAmount;

    private void OnTriggerEnter(Collider other)
    {
        ProcessCollision(other.gameObject);
    }

    private void ProcessCollision(GameObject collider)
    {
        if(collider == PM.gameObject)
        {
            PM.currentWeapon.AddAmmo(ammoAmount);

            Destroy(gameObject);
        }
    }
}
