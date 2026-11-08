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

    [Header("Vision & Detection")]
    public Transform enemyTarget;
    public float visionRane = 25f;
    [Range(0,360)] public float fieldOfView = 90f;
    public LayerMask obstacleMask;
    public Transform eyePosition;
    
    [Header("Movement")]
    public Transform[] patrolPoints;
   
    private int currentPatrolIndex = 0;
    private NavMeshAgent navAgent;

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        if(eyePosition == null)
        {
            eyePosition = transform;
        }
    }

    void Update()
    {
        bool isEnemyVisible = CheckLineOfSight();

        switch (currentState)
        {
            case BotState.Idle:
                Idle(isEnemyVisible);
                break;
            case BotState.Patrol:
                Patrol(isEnemyVisible);
                break;
            case BotState.SeekCover:
                SeekCover(isEnemyVisible);
                break;
            case BotState.Attack:
                Attack(isEnemyVisible);
                break;
            default:
                break;
        }
    }

    private bool CheckLineOfSight()
    {
        if (enemyTarget == null) return false;

        Vector3 directionToEnemy = enemyTarget.position - eyePosition.position;
        float distanceToEnemy = directionToEnemy.magnitude;

        if (distanceToEnemy <= visionRane)
        {
            float angleToEnemy = Vector3.Angle(eyePosition.forward, directionToEnemy);
            if (angleToEnemy <= fieldOfView / 2f)
            {
                if (!Physics.Raycast(eyePosition.position, directionToEnemy.normalized, distanceToEnemy, obstacleMask))
                {
                    Debug.DrawRay(eyePosition.position, directionToEnemy, Color.red);
                    return true;
                }
            }
        }
        return false;
    }
    private void Idle(bool enemyVisible)
    {
        if(patrolPoints.Length > 0)
        {
            currentState = BotState.Patrol;
        }  
    }
    private void Patrol(bool enemyVisible)
    {
        if (patrolPoints.Length == 0) return;
        if (!navAgent.pathPending && navAgent.remainingDistance < 0.5f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            navAgent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }
    private void SeekCover(bool enemyVisible)
    {
        Debug.Log("Bot is Seeking Cover.");
    }
    private void Attack(bool enemyVisible)
    {
        Debug.Log("Bot is Attacking.");
    }
}
