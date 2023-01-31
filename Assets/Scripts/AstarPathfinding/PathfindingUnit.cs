using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingUnit : GameBehaviour
{
    [SerializeField]
    private EnemyBase unit;
    const float minPathUpdateTime = 0.5f;
    const float pathUpdateMoveThreshold = 0.5f;

    public Transform target;
    public float turnDst = 5;
    public float turnSpeed = 3;
    public float stoppingDst = 10;

    //public Path path;
    public Vector3[] path;
    int targetIndex;
    public bool followingPath;
    public float distanceToWaypoint;
    public Vector3 currentWaypoint;
    public Coroutine updatePath;
    private Coroutine followPath;
    public Grid currentGrid;

    [Header("Movement Options")]
    [SerializeField] private bool moveTowardsPlayer;
    [SerializeField] private bool spin;
    [SerializeField] private int verticalDstToMaintain;

    [SerializeField] private float separationStrength = 1f;
    public List<GameObject> objectsToAvoid = new List<GameObject>();
    public List<GameObject> wallsToPlayer = new List<GameObject>();
    public LayerMask groundMask;

    private void Awake()
    {
        target = PM.gameObject.transform;
    }
    public void StartUpdatingPath()
    {
        updatePath = StartCoroutine(UpdatePath());
    }

    public void StopPathfinding()
    {
        if (updatePath != null)
        {
            StopCoroutine(updatePath);
        }
        StopFollowingPath();
    }

    public void ResetPathFinding()
    {
        StopPathfinding();
        StartUpdatingPath();
    }

    public void StopFollowingPath()
    {
        if (followPath != null)
        {
            StopCoroutine(followPath);
        }
    }

    public void CheckPath()
    {
        if (path.Length > 1)
        {
            StopPathfinding();
        }
    }

    private void Update()
    {
        PathfindingMovement();
    }
    public void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            //path = new Path(waypoints, transform.position, turnDst, stoppingDst);
            StopFollowingPath();
            path = waypoints;
            followPath = StartCoroutine(FollowPath());
        }
    }

    public IEnumerator UpdatePath()
    {
        if (Time.timeSinceLevelLoad < 0.3f)
        {
            yield return new WaitForSeconds(0.3f);
        }
        if (currentGrid != null)
        {
            PathRequestManager.RequestPath(new PathRequest(currentGrid, unit.transform.position, target.position, OnPathFound));
            float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
            Vector3 targetPosOld = target.position;

            while (true)
            {
                yield return new WaitForSeconds(minPathUpdateTime);
                if (currentGrid != null)
                {
                    if ((target.position - targetPosOld).sqrMagnitude > sqrMoveThreshold)
                    {
                        PathRequestManager.RequestPath(new PathRequest(currentGrid, unit.transform.position, target.position, OnPathFound));
                        targetPosOld = target.position;
                    }
                }
            }
        }
    }
    private IEnumerator FollowPath()
    {
        targetIndex = 0;
        followingPath = true;
        Vector3 currentWaypoint = path[0];

        while (followingPath)
        {
            if (Vector3.Distance(unit.transform.position, currentWaypoint) < 2)
            {
                targetIndex++;
                if (targetIndex >= path.Length || currentGrid != PM.lastGrid)
                {
                    followingPath = false;
                    CheckWallsToPlayer();
                    break;
                }
                currentWaypoint = path[targetIndex];
            }

            if (followingPath)
            {
                //Quaternion targetRotation = Quaternion.LookRotation(currentWayPoint - transform.position);
                //transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
                //transform.Translate(Vector3.forward * Time.deltaTime * speed);

                unit.transform.position = Vector3.MoveTowards(unit.transform.position, currentWaypoint, unit.moveSpeed * Time.deltaTime);
                unit.transform.LookAt(currentWaypoint);
            }
            yield return null;
        }
    }

    private void CheckWallsToPlayer()
    {
        wallsToPlayer.Clear();

        Ray ray = new Ray(transform.position, PM.gameObject.transform.position);
        if (Physics.Raycast(ray, out RaycastHit hit, unit.sqrDetectionRange, groundMask))
        {
            wallsToPlayer.Add(hit.collider.gameObject);
        }

        if (wallsToPlayer.Count == 1)
        {
            currentGrid = wallsToPlayer[0].GetComponentInChildren<Grid>();
        }
        else if (wallsToPlayer.Count == 0)
        {
            StopPathfinding();
            currentGrid = null;
        }
    }

    private void CollisionAvoidance()
    {
        if (objectsToAvoid != null)
        {
            Vector3 separationForce = Vector3.zero;
            for (int i = 0; i < objectsToAvoid.Count; i++)
            {
                if (objectsToAvoid[i] == null)
                {
                    objectsToAvoid.Remove(objectsToAvoid[i]);
                    continue;
                }
                Vector3 distance = transform.position - objectsToAvoid[i].transform.position;
                separationForce += distance.normalized / distance.magnitude;
            }
            separationForce = separationForce.normalized * separationStrength;
            unit.transform.position += separationForce * Time.deltaTime * unit.moveSpeed;
        }
    }

    private void PathfindingMovement()
    {
        if (spin)
        {
            unit.transform.Rotate(0, 1, 0, Space.Self);
        }
        if (moveTowardsPlayer)
        {
            CollisionAvoidance();

            if (unit.playerDetected)
            {
                if (unit.CanSeePlayer() || currentGrid == null)
                {
                    CheckPath();
                    if (unit.flying)
                    {
                        if(verticalDstToMaintain < unit.verticalDstToPlayer)
                        {
                            //maintain vertical distance to player
                            float verticalDstToMove = verticalDstToMaintain - unit.verticalDstToPlayer;
                            Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y + verticalDstToMove, transform.position.z);
                            unit.transform.position = Vector3.MoveTowards(transform.position, targetPosition, unit.verticalMoveSpeed * Time.deltaTime);
                        }          
                    }
                    //movetowards player
                    if (unit.sqrLenToPlayer > unit.sqrDstToMaintain)
                    {
                        unit.transform.position = Vector3.MoveTowards(transform.position, new Vector3(PM.transform.position.x, transform.position.y, PM.transform.position.z), Time.deltaTime * unit.moveSpeed);
                    }
                }
                else
                {
                    ResetPathFinding();
                }
                if(!unit.CanSeePlayer() && currentGrid == null)
                {
                    //movetowards player
                    if (unit.sqrLenToPlayer > unit.sqrDstToMaintain)
                    {
                        unit.transform.position = Vector3.MoveTowards(transform.position, new Vector3(PM.transform.position.x, transform.position.y, PM.transform.position.z), Time.deltaTime * unit.moveSpeed);
                    }
                }

            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Grid"))
        {
            currentGrid = other.GetComponent<Grid>();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Grid"))
        {
            if (currentGrid == other.GetComponent<Grid>())
            {
                currentGrid = null;
            }
        }
    }
    private void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }

    //private void OnDrawGizmos()
    //{
    //    if (path != null)
    //    {
    //        path.DrawWithGizmos();
    //    }
    //}
}
