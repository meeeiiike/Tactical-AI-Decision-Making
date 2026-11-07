using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class FSMBot : MonoBehaviour
{
    public enum BotState
    {
        Idle,
        Patrol,
        SeekCover,
        Attack
    }

    [Header("FSM State")]
    public BotState currentState = BotState.Idle;

    [Header("Movement")]
    public Transform[] patrolPoints;
    private int currentPatrolIndex = 0;
    private NavMeshAgent navAgent;

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        switch (currentState)
        {
            case BotState.Idle:
                Idle();
                break;
            case BotState.Patrol:
                Patrol();
                break;
            case BotState.SeekCover:
                SeekCover();
                break;
            case BotState.Attack:
                Attack();
                break;
            default:
                break;
        }
    }

    private void Idle()
    {
        if(patrolPoints.Length > 0)
        {
            currentState = BotState.Patrol;
        }  
    }
    private void Patrol()
    {
        if (!navAgent.pathPending && navAgent.remainingDistance < 0.5f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            navAgent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }
    private void SeekCover()
    {
        Debug.Log("Bot is Seeking Cover.");
    }
    private void Attack()
    {
        Debug.Log("Bot is Attacking.");
    }
}
