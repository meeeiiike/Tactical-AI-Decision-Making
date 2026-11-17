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
    public float visionRange = 25f;
    [Range(0,360)] public float fieldOfView = 90f;
    public LayerMask obstacleMask;
    public Transform eyePosition;
    
    [Header("Movement")]
    public Transform[] patrolPoints;
    private int currentPatrolIndex = 0;
    private NavMeshAgent navAgent;

    [Header("Cover")]
    public float coverSearchRadius = 10f;
    public float hideBehindOffset = 1f;
    public bool hasCalculatedCover = false;

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

        if (distanceToEnemy <= visionRange)
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
        if(enemyVisible)
        {
            currentState = BotState.Attack;
            return;
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
        if(enemyVisible)
        {
            currentState = BotState.Attack;
            return;
        }
    
    }
    private void SeekCover(bool enemyVisible)
    {
        if(!hasCalculatedCover)
        {
            Vector3 coverPosition = CalculateNearestCover();
            if (coverPosition != Vector3.zero)
            {
                navAgent.isStopped = false;
                navAgent.SetDestination(coverPosition);
                hasCalculatedCover = true;
            }
            else
            {
                currentState = BotState.Attack;
                return;
            }
        }

        if(!navAgent.pathPending && navAgent.remainingDistance < 0.5f)
        {
            hasCalculatedCover = false;
            if(!enemyVisible)
            {
                currentState = BotState.Idle;
                return;
            }
            else
            {
                currentState = BotState.Attack;
                return;
            }
        }
    }
            
    
    private void Attack(bool enemyVisible)
    {
        if(!enemyVisible)
        {
            navAgent.isStopped = false;
            currentState = BotState.Idle;
            return;
        }
        navAgent.isStopped = true;

        float simHealth = 40f;
        if(simHealth < 50f)
        {
            hasCalculatedCover = false;
            currentState = BotState.SeekCover;
            return;
        }
    }

    private Vector3 CalculateNearestCover()
    {
        Collider[] coverColliders = Physics.OverlapSphere(transform.position, coverSearchRadius, obstacleMask);
        Vector3 nearestCover = Vector3.zero;
        float nearestDistance = Mathf.Infinity;

        foreach(Collider cover in coverColliders)
        {
            Vector3 directionFromEnemy = (cover.transform.position - enemyTarget.position).normalized;
            Vector3 coverPosition = cover.transform.position + (directionFromEnemy * hideBehindOffset);
            NavMeshHit hit;
            if(NavMesh.SamplePosition(coverPosition, out hit, 1.0f, NavMesh.AllAreas))
            {
                float distanceToCover = Vector3.Distance(transform.position, hit.position);
                if(distanceToCover < nearestDistance)
                {
                    nearestDistance = distanceToCover;
                    nearestCover = hit.position;
                }
            }
        }
        return nearestCover;
    }
}

