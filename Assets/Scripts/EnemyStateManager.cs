using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))] // Ensure Animator is present
public class EnemyStateManager : MonoBehaviour
{
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
    public Transform[] patrolPoints;
    private int currentPatrolIndex = 0;

    private NavMeshAgent agent;
    private Animator animator; // ✅ Add animator reference

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>(); // ✅ Get animator
        agent.updateRotation = false; // We'll handle rotation manually

        SetState(EnemyState.Patrol);
    }

    private void Update()
    {
        switch (currentState)
        {
            case EnemyState.Patrol:
                Debug.Log("🔵 Patrol State");
                Patrol();
                break;

            case EnemyState.Chase:
                Debug.Log("🔴 Chase State");
                break;

            case EnemyState.Attack:
                Debug.Log("🟡 Attack State");
                break;

            case EnemyState.Search:
                Debug.Log("🟠 Search State");
                break;

            case EnemyState.Retreat:
                Debug.Log("⚫ Retreat State");
                break;
        }
    }

    public void SetState(EnemyState newState)
    {
        if (currentState != newState)
        {
            Debug.Log($"State changed from {currentState} to {newState}");
            currentState = newState;

            // ✅ Update animation based on state
            switch (newState)
            {
                case EnemyState.Patrol:
                    // animator.SetBool("isWalking", true);
                    // animator.SetBool("isRunning", false);
                    // animator.SetBool("isAttacking", false);
                    break;

                case EnemyState.Chase:
                    // animator.SetBool("isWalking", false);
                    // animator.SetBool("isRunning", true);
                    break;

                case EnemyState.Attack:
                    // animator.SetBool("isWalking", false);
                    // animator.SetBool("isRunning", false);
                    // animator.SetTrigger("attack");
                    break;

                default:
                    // animator.SetBool("isWalking", false);
                    // animator.SetBool("isRunning", false);
                    break;
            }
        }
    }
private void Patrol()
{
    if (patrolPoints.Length == 0 || agent == null) return;

    // Move to next point if close enough
    if (!agent.pathPending && agent.remainingDistance < 0.5f)
    {
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
    }

    // Rotate to face direction of movement
    if (agent.velocity.sqrMagnitude > 0.1f)
    {
        Quaternion targetRotation = Quaternion.LookRotation(agent.velocity.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
    }

    // Update animation speed parameter
    float speed = agent.velocity.magnitude;
    animator.SetFloat("Speed", speed);
}



}
