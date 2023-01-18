using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingBullet : GameBehaviour
{
    public float speed = 10f;
    public GameObject target;
    private Rigidbody rb;

    private float maxSpeed = 50f;

    private float findTargetWaitTime = 1;
    public bool searchForTarget;

    private float rotationThreshold = 100f;
    private float rotationSpeed = 100f;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }


    private void Start()
    {
        findTargetWaitTime = Mathf.Clamp(findTargetWaitTime, 0, findTargetWaitTime);
        ValidateValues();

    }
    private void Update()
    {
        findTargetWaitTime -= Time.deltaTime;

        if (findTargetWaitTime <= 0)
        {
            searchForTarget = true;
        }
        if (searchForTarget)
        {
            MoveTowardsTarget();
        }
        ClampSpeed();
    }
    private void ValidateValues()
    {
        searchForTarget = false;
        if (speed == 0)
        {
            speed = 100;
        }
        target = null;
    }
    private void ClampSpeed()
    {
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
        }
    }

    void MoveTowardsTarget()
    {
        if (target == null)
        {
            GetClosestEnemy();
        }
        if (target != null)
        {
           
            // Rotate the missile towards the target
            transform.LookAt(target.transform);
            Vector3 directionToTarget = target.transform.position - transform.position;
           
            if (Vector3.Distance(transform.position, target.transform.position) <= 10f)
            {
                rb.angularVelocity = Vector3.zero;
                rb.AddForce(directionToTarget * maxSpeed);
                return;
            }
            // Apply the force to the missile in the direction it is facing
            rb.AddForce(directionToTarget * speed);
        }
    }
    private void GetClosestEnemy()
    {
        target = null;

        float minDist = Mathf.Infinity;


        if (SM.enemiesAlive.Count > 0)
        {
            for (int i = 0; i < SM.enemiesAlive.Count; i++)
            {
                GameObject enemy = SM.enemiesAlive[i];
                float dist = Vector3.Distance(enemy.transform.position, transform.position);
                if (dist < minDist)
                {
                    target = enemy;
                    minDist = dist;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if(target!= null)
        {
        Gizmos.DrawLine(transform.position,target.transform.position);

        }
    }

}
