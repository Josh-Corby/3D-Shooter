using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAvoidance : MonoBehaviour
{
    private EnemyBase unit;
    public List<GameObject> collisionObjects = new List<GameObject>();
    private GameObject closestObject;
    private int avoidanceDistance = 5;

    private void Awake()
    {
        unit = GetComponentInParent<EnemyBase>();
    }


    //private void FindClosestObject()
    //{
    //    closestObject = null;
    //    foreach (GameObject collisionObject in collisionObjects)
    //    {
    //        float closestDst = Mathf.Infinity;

    //        float distance = Vector3.Distance(transform.position, collisionObject.transform.position);
    //        if (distance < closestDst)
    //        {
    //            closestDst = distance;
    //            closestObject = collisionObject;
    //        }
    //    }   
    //}

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == unit.gameObject)
        {
            return;
        }

        if (other.gameObject.CompareTag("PlayerAttack") || other.gameObject.CompareTag("EnemyAttack") || other.CompareTag("Mechanics"))
        {
            return;
        }
        collisionObjects.Add(other.gameObject);
        unit.objectsToAvoid.Add(other.gameObject);
        //FindClosestObject();
    }
    private void OnTriggerExit(Collider other)
    {
        if (collisionObjects.Contains(other.gameObject))
        {
            collisionObjects.Remove(other.gameObject);
            unit.objectsToAvoid.Remove(other.gameObject);
            //FindClosestObject();
        }
    }
}
