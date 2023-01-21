using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Transform target;
    public float speed;
    Vector3[] path;
    int targetIndex;

    private void Start()
    {
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            StopCoroutine(FollowPath());
            StartCoroutine(FollowPath());
        }
    }

    IEnumerator FollowPath()
    {
        Vector3 currenWaypoint = path[0];
        while (true)
        {
            if(transform.position == currenWaypoint)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    yield break;
                }
                currenWaypoint = path[targetIndex];
            }
            transform.position = Vector3.MoveTowards(transform.position, currenWaypoint, speed * Time.deltaTime);
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

                if(i == targetIndex)
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
}
