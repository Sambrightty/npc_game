using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Manages the enemy's current state and handles patrol behavior.
/// Other scripts should call SetState to trigger transitions.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyStateManager : MonoBehaviour
{
    /// <summary>
    /// Possible states for the enemy FSM.
    /// </summary>
    public enum EnemyState
    {
        Patrol,
        Chase,
        Attack,
        Search,
        Retreat
    }

    [Header("State Management")]
    public EnemyState currentState = EnemyState.Patrol;

    [Header("Patrol Settings")]
    public Transform[] patrolPoints;           // Points to walk between during patrol
    private int currentPatrolIndex = 0;

    private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Begin in Patrol state
        SetState(EnemyState.Patrol);
    }

    private void Update()
    {
        // Debug current state
        switch (currentState)
        {
            case EnemyState.Patrol:
                Debug.Log("🔵 Patrol State");
                Patrol();
                break;

            case EnemyState.Chase:
                Debug.Log("🔴 Chase State");
                // Logic handled by EnemyFSM
                break;

            case EnemyState.Attack:
                Debug.Log("🟡 Attack State");
                // Logic handled by EnemyFSM
                break;

            case EnemyState.Search:
                Debug.Log("🟠 Search State");
                // Logic handled by EnemyFSM
                break;

            case EnemyState.Retreat:
                Debug.Log("⚫ Retreat State");
                // Logic handled by EnemyFSM
                break;
        }
    }

    /// <summary>
    /// Sets the enemy's current FSM state.
    /// </summary>
    /// <param name="newState">The new state to transition into.</param>
    public void SetState(EnemyState newState)
    {
        if (currentState != newState)
        {
            Debug.Log($"State changed from {currentState} to {newState}");
            currentState = newState;
        }
    }

    /// <summary>
    /// Patrol behavior: moves between patrol points in sequence.
    /// </summary>
    private void Patrol()
    {
        if (patrolPoints.Length == 0 || agent == null) return;

        // If close to current patrol target, go to next point
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }
}
