using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingBullet : BulletBase
{
    private Rigidbody rb;
    private readonly float maxSpeed = 50f;
    private float findTargetWaitTime = 1;
    public bool searchForTarget;
    private GameObject targetObject;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void Start()
    {
        findTargetWaitTime = Mathf.Clamp(findTargetWaitTime, 0, findTargetWaitTime);
        ValidateValues();
        base.Start();
        targetObject = null;

    }
    public override void Update()
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
    public override void ValidateValues()
    {
        searchForTarget = false;
        if (speed == 0)
        {
            speed = 10;
        }
        base.ValidateValues();
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
        if (targetObject == null)
        {
            GetClosestEnemy();
        }
        if (targetObject != null)
        {
           
            // Rotate the missile towards the target
            transform.LookAt(target);
            Vector3 directionToTarget = targetObject.transform.position - transform.position;
           
            if (Vector3.Distance(transform.position, targetObject.transform.position) <= 30f)
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
        float minDist = Mathf.Infinity;

        if (SM.enemiesAlive.Count > 0)
        {
            for (int i = 0; i < SM.enemiesAlive.Count; i++)
            {
                GameObject enemy = SM.enemiesAlive[i];
                float dist = Vector3.Distance(enemy.transform.position, transform.position);
                if (dist < minDist)
                {
                    targetObject = enemy;
                    minDist = dist;
                }
            }
        }
    }
    private void OnDrawGizmos()
    {
        if(targetObject != null)
        {
            Gizmos.DrawLine(transform.position, targetObject.transform.position);
        }
    }
}
