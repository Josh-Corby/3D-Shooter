using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : GameBehaviour
{
    private Room parentRoom;

    private void Awake()
    {
        parentRoom = GetComponentInParent<Room>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == PM.gameObject)
        {
            parentRoom.CheckCombatCleared();
        }
    }

}
