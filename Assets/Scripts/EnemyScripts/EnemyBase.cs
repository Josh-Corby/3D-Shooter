using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : GameBehaviour, IDamagable
{
    public float maxHealth = 10;
    [SerializeField] private float currentHealth;
    [SerializeField] private EnemyHealthBar healthBar;

    [SerializeField] private float fireRange;
    private float sqrFireRange;
    [SerializeField] private float detectionRange;
    [HideInInspector]
    public float sqrDetectionRange;
    public bool playerDetected;
    public bool playerInFireRange;
    [SerializeField] private bool lookAtPlayer;
    public float moveSpeed;
    public float verticalMoveSpeed;
    [SerializeField] private int dstToMaintain;
    [HideInInspector] public int sqrDstToMaintain;
    private Vector3 dstToPlayer;
    [HideInInspector] public float sqrLenToPlayer;
    [HideInInspector] public float verticalDstToPlayer;

    private Vector3 playerPosition;

    public bool flying;

    public LayerMask detectionMask;
    public bool canSeePlayer;

    private void Awake()
    {
        healthBar = GetComponentInChildren<EnemyHealthBar>();
    }
    private void Start()
    {
        currentHealth = maxHealth;
        sqrFireRange = fireRange * fireRange;
        sqrDetectionRange = detectionRange * detectionRange;
        sqrDstToMaintain = dstToMaintain * dstToMaintain;
    }

    private void Update()
    {
        playerPosition = PM.gameObject.transform.position;
        dstToPlayer = transform.position - playerPosition;
        sqrLenToPlayer = dstToPlayer.sqrMagnitude;
        verticalDstToPlayer = transform.position.y - playerPosition.y;

        if (sqrLenToPlayer < sqrDetectionRange)
        {
            playerDetected = true;

            if (lookAtPlayer)
            {
                if (flying)
                {
                    transform.LookAt(playerPosition);
                }
                else
                {
                    Vector3 targetPosition = new Vector3(playerPosition.x, transform.position.y, playerPosition.z);
                    transform.LookAt(targetPosition);
                }          
            }
        }

        if (sqrLenToPlayer < sqrFireRange)
        {
            playerInFireRange = true;
        }
    }

    public bool CanSeePlayer()
    {
        Vector3 dirToPlayer = PM.gameObject.transform.position - transform.position;
        Ray ray = new Ray(transform.position, dirToPlayer);
        if (Physics.SphereCast(transform.position, 1, dirToPlayer, out RaycastHit hit, detectionMask, detectionMask))
        {
            if (hit.collider.CompareTag("Player"))
            {
                canSeePlayer = true;
            }
            else
            {
                canSeePlayer = false;
            }
        }
        //Debug.Log(canSeePlayer);
        return canSeePlayer;
    }


    public void Damage(float damage)
    {
        currentHealth -= damage;
        Debug.Log("Enemy hit");
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            healthBar.SetHP(currentHealth);
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
}
