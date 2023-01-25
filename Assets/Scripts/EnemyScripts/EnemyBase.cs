using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : GameBehaviour
{
    private float maxHealth = 10;
    [SerializeField] private float currentHealth;
    [SerializeField] private float fireRange;
    private float sqrFireRange;
    [SerializeField] private float detectionRange;
    private float sqrDetectionRange;
    public bool playerDetected;
    public bool playerInFireRange;
    [SerializeField] private bool lookAtPlayer;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float verticalMoveSpeed;
    [SerializeField] private int dstToMaintain;
    private int sqrDstToMaintain;
    private Vector3 dstToPlayer;
    private float sqrLenToPlayer;
    private float verticalDstToPlayer;

    [Header("Movement Options")]
    [SerializeField] private bool moveTowardsPlayer;
    [SerializeField] private bool spin;
    public bool flying;
    [SerializeField] private int verticalDstToMaintain;

    public List<GameObject> objectsToAvoid = new List<GameObject>();
    [SerializeField] private float separationStrength = 1f;

    private PathfindingUnit pathfinding;
    public LayerMask detecionMask;
    private bool canSeePlayer;
    [SerializeField]
    private bool isPathfinding;


    private void Awake()
    {
        pathfinding = GetComponentInChildren<PathfindingUnit>();
    }
    private void Start()
    {
        isPathfinding = false;
        currentHealth = maxHealth;
        sqrFireRange = fireRange * fireRange;
        sqrDetectionRange = detectionRange * detectionRange;
        sqrDstToMaintain = dstToMaintain * dstToMaintain;
    }

    private void Update()
    {

        dstToPlayer = transform.position - PM.gameObject.transform.position;
        sqrLenToPlayer = dstToPlayer.sqrMagnitude;
        verticalDstToPlayer = transform.position.y - PM.gameObject.transform.position.y;
        EnemyMovement();

        if (sqrLenToPlayer < sqrDetectionRange)
        {
            playerDetected = true;

            if (lookAtPlayer)
                transform.LookAt(PM.gameObject.transform.position);
        }
        if (sqrLenToPlayer < sqrFireRange)
        {
            playerInFireRange = true;
        }
    }

    protected void CollisionAvoidance()
    {
        if (objectsToAvoid != null)
        {
            Vector3 separationForce = Vector3.zero;
            foreach (GameObject objectToAvoid in objectsToAvoid)
            {
                Vector3 distance = transform.position - objectToAvoid.transform.position;
                separationForce += distance.normalized / distance.magnitude;
            }
            separationForce = separationForce.normalized * separationStrength;
            transform.position += separationForce * Time.deltaTime * moveSpeed;
        }
    }

    private bool CanSeePlayer()
    {
        Vector3 dirToPlayer = PM.gameObject.transform.position - transform.position;
        Ray ray = new Ray(transform.position, dirToPlayer);
        if(Physics.SphereCast(transform.position,1,dirToPlayer, out RaycastHit hit, detecionMask, detecionMask))
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
    protected void EnemyMovement()
    {
        if (spin)
        {
            transform.Rotate(0, 1, 0, Space.Self);
        }
        if (moveTowardsPlayer)
        {
            if (!isPathfinding)
            {
                CollisionAvoidance();
            }

            if (playerDetected)
            {
                if (CanSeePlayer() || pathfinding.currentGrid != PM.lastGrid)
                {
                    if(pathfinding.path.Length >0)
                    {
                        pathfinding.StopUpdatingPath();
                        pathfinding.StopFollowingPath();
                        isPathfinding = false;
                    }
                    if (flying)
                    {
                        //maintain vertical distance to player
                        float verticalDstToMove = verticalDstToMaintain - verticalDstToPlayer;
                        Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y + verticalDstToMove, transform.position.z);
                        float interpolationFactor = Mathf.Abs(verticalDstToMove / verticalDstToPlayer);
                        transform.position = Vector3.Lerp(transform.position, targetPosition, interpolationFactor / 100);
                    }
                    //movetowards player
                    if (sqrLenToPlayer > sqrDstToMaintain)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, new Vector3(PM.transform.position.x, transform.position.y, PM.transform.position.z), Time.deltaTime * moveSpeed);
                    }
                }
                else
                {
                    if (!isPathfinding)
                    {
                        pathfinding.ResetPathFinding();
                        isPathfinding = true;
                    }
                }

            }        
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


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, fireRange);
    }
}
