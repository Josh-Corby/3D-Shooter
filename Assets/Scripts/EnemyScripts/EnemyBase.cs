using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : GameBehaviour
{
    private enum MovementTypes
    {
        None, MoveTowardsPlayer, Spin, SpinTowardsPlayer
    }

    public enum EnemyType
    {
        Grounded,
        Flying
    }
    [SerializeField]
    private MovementTypes moveType;
    public EnemyType type;

    private float maxHealth = 10;
    [SerializeField] private float currentHealth;
    [SerializeField] private float fireRange;
    [SerializeField] private float detectionRange;
    public bool playerDetected;
    public bool playerInFireRange;
    [SerializeField] private bool lookAtPlayer;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        EnemyMovement();

        Vector3 distanceToPlayer = (transform.position - PM.gameObject.transform.position);
        float sqrLen = distanceToPlayer.sqrMagnitude;

        if (sqrLen < detectionRange * detectionRange)
        {
            playerDetected = true;

            if (lookAtPlayer)
                transform.LookAt(PM.gameObject.transform.position);
        }
        if (sqrLen < fireRange * fireRange)
        {
            playerInFireRange = true;
        }
    }

    protected void EnemyMovement()
    {
        switch (moveType)
        {
            case (MovementTypes.Spin):
                transform.Rotate(0, 1, 0, Space.Self);
                break;
        }

    }
    public void Damage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (SM.enemiesAlive.Contains(gameObject))
        {
            SM.enemiesAlive.Remove(gameObject);
        }
        Destroy(gameObject);
    }

    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawWireSphere(transform.position, detectionRange);
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, fireRange);
    //}


    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawLine(transform.position, player.transform.position);
    //}
}
