using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingUnit : GameBehaviour
{
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

    private Coroutine routine;

    private void Start()
    {
        StartCoroutine(UpdatePath());
    }


    public void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            //path = new Path(waypoints, transform.position, turnDst, stoppingDst);
            if (routine != null)
            {
                StopCoroutine(routine);

            }
            path = waypoints;
            routine = StartCoroutine(FollowPath());
        }
    }

    public IEnumerator UpdatePath()
    {

        if (Time.timeSinceLevelLoad < 0.3f)
        {
            yield return new WaitForSeconds(0.3f);
        }
        PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));

        float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
        Vector3 targetPosOld = target.position;

        while (true)
        {
            yield return new WaitForSeconds(minPathUpdateTime);
            if ((target.position - targetPosOld).sqrMagnitude > sqrMoveThreshold)
            {
                PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));
                targetPosOld = target.position;
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
            if (transform.position == currentWaypoint)
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

                transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
                transform.LookAt(currentWaypoint);
            }
            yield return null;
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
