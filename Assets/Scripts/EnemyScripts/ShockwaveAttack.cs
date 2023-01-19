using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockwaveAttack : GameBehaviour
{
    private LineRenderer line;
    private MeshCollider col;
    private int segments = 100;
    private float radius;
    [SerializeField] private int maxRadius = 20;
    [SerializeField] private float expandRate;
    private bool expanding;
    [SerializeField] private int attackCooldownMax;
    private float attackCooldown;
    private bool attackFinished;

    private void Start()
    {
        attackCooldown = attackCooldownMax;
        line = GetComponent<LineRenderer>();
        line.positionCount = segments + 1;
        line.useWorldSpace = false;
        line.enabled = false;
        col = GetComponent<MeshCollider>();
        StartCoroutine(StartAttack());
    }

    private void Update()
    {

        if (expanding)
        {
            radius += expandRate;
            CreatePoints();
        }
        if (radius >= maxRadius && !attackFinished)
        {
            ResetAttack();
        }
        if (attackFinished)
        {
            attackCooldown -= Time.deltaTime;
            if (attackCooldown <= 0)
            {
                StartCoroutine(StartAttack());
            }
        }
    }
    private IEnumerator StartAttack()
    {
        expanding = true;
        attackFinished = false;

        radius = 0;
        attackCooldown = attackCooldownMax;

        //wait till first call off attack to turn line and collider back on so shape can be reset
        yield return new WaitForEndOfFrame();
        col.enabled = true;
        line.enabled = true;
    }
    private void ResetAttack()
    {
        expanding = false;
        attackFinished = true;
        line.enabled = false;
        col.enabled = false;
    }

    private void CreatePoints()
    {
        float x;
        float y = 0f;
        float z;

        float angle = 20f;

        for (int i = 0; i < segments + 1; i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            z = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

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