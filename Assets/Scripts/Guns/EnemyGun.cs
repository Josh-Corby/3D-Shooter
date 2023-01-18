using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGun : GunBase
{
    private Enemy enemy;

    protected override void Awake()
    {
        base.Awake();
        enemy = GetComponentInParent<Enemy>();
    }
    protected override void Update()
    {
        if (enemy.playerDetected)
        {
            transform.LookAt(PM.gameObject.transform);
            CheckForShootCondition();
        }

        if (enemy.playerInRange)
        {
            CheckForShootCondition();
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
            FindTarget();
            StartCoroutine(ResetShooting());
        }
    }
}
