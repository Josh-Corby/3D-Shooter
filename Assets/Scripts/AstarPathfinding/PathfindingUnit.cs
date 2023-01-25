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
    public float speed;
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


    public void StartUpdatingPath()
    {
        updatePath = StartCoroutine(UpdatePath());
    }

    public void StopUpdatingPath()
    {
        if (updatePath != null)
        {
            StopCoroutine(updatePath);
        }
    }

    public void ResetPathFinding()
    {
        StopUpdatingPath();
        StartUpdatingPath();
    }

    public void StopFollowingPath()
    {
        if (followPath != null)
        {
            StopCoroutine(followPath);

        }
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
        if (currentGrid != null && currentGrid == PM.lastGrid)
        {
            if (Time.timeSinceLevelLoad < 0.3f)
            {
                yield return new WaitForSeconds(0.3f);
            }
            PathRequestManager.RequestPath(new PathRequest(currentGrid, unit.transform.position, target.position, OnPathFound));

            float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
            Vector3 targetPosOld = target.position;

            while (true)
            {
                yield return new WaitForSeconds(minPathUpdateTime);
                if ((target.position - targetPosOld).sqrMagnitude > sqrMoveThreshold)
                {
                    PathRequestManager.RequestPath(new PathRequest(currentGrid, unit.transform.position, target.position, OnPathFound));
                    targetPosOld = target.position;
                }
            }
        }
    }
    IEnumerator FollowPath()
    {
        targetIndex = 0;
        followingPath = true;
        Vector3 currentWaypoint = path[0];

        while (followingPath)
        {
            if (unit.transform.position == currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    followingPath = false;
                    break;
                }
                currentWaypoint = path[targetIndex];
            }

            if (followingPath)
            {
                //Quaternion targetRotation = Quaternion.LookRotation(currentWayPoint - transform.position);
                //transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
                //transform.Translate(Vector3.forward * Time.deltaTime * speed);

                unit.transform.position = Vector3.MoveTowards(unit.transform.position, currentWaypoint, speed * Time.deltaTime);
                unit.transform.LookAt(currentWaypoint);
            }
            yield return null;
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
            if (other.TryGetComponent<Grid>(out _) == currentGrid)
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
