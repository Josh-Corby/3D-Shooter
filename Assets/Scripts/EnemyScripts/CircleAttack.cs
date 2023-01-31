using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleAttack : BulletBase
{
    private LineRenderer line;
    private MeshCollider col;

    private int segments = 40;
    [SerializeField] private float radius;
    [SerializeField] private float speed;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = segments + 1;
        line.useWorldSpace = false;
        line.enabled = true;
        col = GetComponent<MeshCollider>();
    }

    public override void Start()
    {
        CreatePoints();
    }

    private void Update()
    {
        transform.position += transform.forward * Time.deltaTime * speed;
    }
    private void CreatePoints()
    {
        float x;
        float y;
        float z = 0f;

        float angle = 20f;

        for (int i = 0; i < segments + 1; i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

            line.SetPosition(i, new Vector3(x, y, z));
            angle += (360f / segments);
        }
        //Connect the last point to the first point
        line.SetPosition(segments, line.GetPosition(0));
        GenerateMeshCollider();
    }

    private void GenerateMeshCollider()
    {
        Mesh mesh = new Mesh();
        line.BakeMesh(mesh);
        col.sharedMesh = mesh;
    }
}
