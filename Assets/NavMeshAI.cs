using UnityEngine;
using UnityEngine.AI;

public class NavMeshAI : GameBehaviour
{
    public Transform goal;
    NavMeshAgent agent;
    private EnemyBase unit;
    private float wanderTimer = 2;
    private float wanderRadius = 20;
    public float timer;


    

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        unit = GetComponent<EnemyBase>();
        goal = PM.gameObject.transform;
    }

    private void Start()
    {
        agent.speed = unit.moveSpeed;
        timer = wanderTimer;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if(timer >= wanderTimer)
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            agent.SetDestination(newPos);
            timer = 0;
        }

        //CheckDistance();
        //agent.destination = goal.position;
    }

    private Vector3 RandomNavSphere(Vector3 origin, float dst, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dst;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dst, layermask);

        return navHit.position;
    }

    private void CheckDistance()
    {
        if(unit.sqrDstToMaintain > unit.sqrLenToPlayer)
        {
            agent.isStopped = true;
        }

        else
        {
            agent.isStopped = false;
            
        }
    }

    private void GetWanderPosition()
    {

    }
}