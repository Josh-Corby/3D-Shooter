using UnityEngine;
using UnityEngine.AI;

public class NavMeshAI : MonoBehaviour
{
    public Transform goal;
    NavMeshAgent agent;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        agent.destination = goal.position;
    }
}