using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGun : GunBase
{
    private EnemyBase enemy;

    [SerializeField]
    private bool shootAtPlayer;

    protected override void Awake()
    {
        base.Awake();
        enemy = GetComponentInParent<EnemyBase>();
    }

    protected override void Update()
    {
        if (enemy.playerDetected)
        {
            if (shootAtPlayer)
            {
                transform.LookAt(PM.gameObject.transform);
            }
            FindTarget();
        }

        if (enemy.playerInFireRange)
        {
            CheckForShootCondition();         
        }
    }

    protected override void FindTarget()
    {
        if (shootAtPlayer)
        {
            targetPoint = PM.gameObject.transform.position;
        }

        if (!shootAtPlayer)
        {
            Ray ray = new Ray(firePointTransform.position, transform.forward);
            //check if ray hits something
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, mask))
            {
                distanceToTarget = hit.distance;
                targetPoint = hit.point;
            }
            else
            {
                targetPoint = ray.GetPoint(1000);
            }
        }
    }
    protected override void CheckForShootCondition()
    {
        if (readyToFire && burstFire)
        {
            StartCoroutine(BurstFire());
        }

        else if (readyToFire)
        {
            CheckFireType();
            StartCoroutine(ResetShooting());
        }
    }
}