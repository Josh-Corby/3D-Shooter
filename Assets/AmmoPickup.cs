using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
