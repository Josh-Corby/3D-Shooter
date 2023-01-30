using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAvoidance : MonoBehaviour
{
    private EnemyBase unit;
    private void Awake()
    {
        unit = GetComponentInParent<EnemyBase>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == unit.gameObject)
        {
            return;
        }

        if (other.gameObject.CompareTag("PlayerAttack") || other.gameObject.CompareTag("EnemyAttack") || other.CompareTag("Mechanics") || other.CompareTag("Grid"))
        {
            return;
        }
        unit.objectsToAvoid.Add(other.gameObject);
    }
    private void OnTriggerExit(Collider other)
    {
        if (unit.objectsToAvoid.Contains(other.gameObject))
        {
            unit.objectsToAvoid.Remove(other.gameObject);
        }
    }
}
