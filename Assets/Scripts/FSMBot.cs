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
        Debug.Log("Bot is idling.");
    }
    private void Patrol()
    {
        Debug.Log("Bot is Patrolling.");
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
