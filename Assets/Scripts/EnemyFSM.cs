using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(EnemyStateManager))]
public class EnemyFSM : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public FieldOfView fieldOfView;
    public HealthSystem healthSystem;
    public PlayerBehaviorTracker behaviorTracker;

    private NavMeshAgent agent;
    private EnemyStateManager stateManager;
    private GeneticEnemyAI enemyAI;
    private EnemyGrudgeMemory grudgeMemory;
    private VoiceManager voiceManager;
    private Animator animator;

    [Header("Retreat Settings")]
    public float healingRate = 5f;
    public Transform retreatPoint; // <- ADD THIS


    [Header("State Thresholds")]
    public float chaseDistance = 10f;
    public float attackDistance = 2f;
    public float lowHealthThreshold = 30f;

    [Header("Search Settings")]
    public float searchDuration = 5f;

    // [Header("Retreat Settings")]
    // public float healingRate = 5f;

    private float attackCooldown = 1.5f;
    private float attackTimer;
    private float searchTimer;

    private bool isSearching = false;
    private bool isRetreating = false;
    private bool isHealing = false;
    private Vector3 retreatTarget;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        stateManager = GetComponent<EnemyStateManager>();
        grudgeMemory = GetComponent<EnemyGrudgeMemory>();
        enemyAI = GetComponent<GeneticEnemyAI>();
        voiceManager = GetComponent<VoiceManager>();
        animator = GetComponentInChildren<Animator>();

        ApplyGrudgeBehavior();
        ValidatePlayerHealthSystem();
    }

    private void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool canSeePlayer = fieldOfView.CanSeePlayer;

        animator.SetFloat("Speed", agent.velocity.magnitude);
        ResetBlockingIfNotAttacking();

        if (healthSystem.currentHealth < lowHealthThreshold)
        {
            stateManager.SetState(EnemyStateManager.EnemyState.Retreat);
            Retreat();
        }
        else if (canSeePlayer && distanceToPlayer <= attackDistance)
        {
            EngageOrAttack(distanceToPlayer);
        }
        else if (canSeePlayer && distanceToPlayer <= chaseDistance)
        {
            TransitionToChase();
        }
        else if (!canSeePlayer && IsInCombatState())
        {
            StartSearch();
        }
        else if (isSearching)
        {
            HandleSearchTimer();
        }

        RotateTowardsMovement();
    }

    #region --- Combat Logic ---

    private void ResetBlockingIfNotAttacking()
    {
        if (stateManager.currentState != EnemyStateManager.EnemyState.Attack)
            SetBlocking(false);
    }

    private void EngageOrAttack(float distanceToPlayer)
    {
        if (distanceToPlayer > 1.5f)
        {
            agent.SetDestination(player.position);
        }
        else
        {
            agent.SetDestination(transform.position);
            EndSearch();
            stateManager.SetState(EnemyStateManager.EnemyState.Attack);
            Attack();
        }
    }

    private void Attack()
    {
        agent.SetDestination(transform.position);
        SetBlocking(false);

        attackTimer += Time.deltaTime;
        if (attackTimer < attackCooldown) return;
        attackTimer = 0f;

        string action = enemyAI.DecideNextAction();
        switch (action)
        {
            case "Attack":
                TryAttackPlayer();
                break;
            case "Block":
                TryStartBlock();
                break;
            case "Dodge":
                Debug.Log("🌀 Enemy dodges!");
                break;
            default:
                Debug.Log("😐 Enemy does nothing.");
                break;
        }

        TrackAndEvolveBehavior();
    }

    private void TryAttackPlayer()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > attackDistance)
        {
            Debug.Log("⚠️ Player is out of attack range.");
            return;
        }

        HealthSystem playerHealth = player.GetComponent<HealthSystem>();
        if (playerHealth == null)
        {
            Debug.LogError("🚨 No HealthSystem found on player!");
            return;
        }

        if (!playerHealth.IsBlocking)
        {
            animator.SetTrigger("punch");
            playerHealth.TakeDamage(10f);
            Debug.Log("👊 Enemy punches the player!");
        }
        else
        {
            Debug.Log("🛡️ Player blocked the punch!");
        }
    }

    private void TryStartBlock()
    {
        if (Vector3.Distance(transform.position, player.position) <= attackDistance + 0.5f)
        {
            StartBlock();
        }
        else
        {
            Debug.Log("❌ Blocking skipped: player too far.");
        }
    }

    private void StartBlock()
    {
        SetBlocking(true);
        Debug.Log("🛡️ Enemy blocks!");
        Invoke(nameof(StopBlocking), 1f);
    }

    private void StopBlocking()
    {
        SetBlocking(false);
    }

    private void SetBlocking(bool isBlocking)
    {
        healthSystem.IsBlocking = isBlocking;
        animator.SetBool("isBlocking", isBlocking);
    }

    private void TrackAndEvolveBehavior()
    {
        if (behaviorTracker == null) return;

        string behavior = behaviorTracker.GetBehaviorType();
        if (!string.IsNullOrEmpty(behavior))
        {
            enemyAI.EvolveGene(behavior);
            behaviorTracker.ResetBehavior();
        }
    }

    #endregion

    #region --- Retreat & Healing ---

    private void Retreat()
    {
        if (!isRetreating)
        {
            if (retreatPoint != null)
{
    retreatTarget = retreatPoint.position;
}
else
{
    Debug.LogWarning("⚠️ Retreat point not set! Falling back to dynamic retreat.");
    Vector3 directionAway = (transform.position - player.position).normalized;
    retreatTarget = transform.position + directionAway * 10f;
}


            if (NavMesh.SamplePosition(retreatTarget, out NavMeshHit hit, 10f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                isRetreating = true;
                voiceManager?.PlayRetreatVoice();
            }
        }
        else if (Vector3.Distance(transform.position, retreatTarget) < 1.5f && !isHealing)
        {
            isHealing = true;
            StartCoroutine(HealOverTime());
        }
    }

   private IEnumerator HealOverTime()
{
    Debug.Log("💊 Enemy starts healing...");

    while (healthSystem.currentHealth < healthSystem.maxHealth)
    {
        healthSystem.Heal(healingRate * Time.deltaTime);
        yield return null;
    }

    Debug.Log("✅ Enemy fully healed. Returning to patrol.");
    isHealing = false;
    isRetreating = false;
    stateManager.SetState(EnemyStateManager.EnemyState.Patrol);
}


    #endregion

    #region --- Searching ---

    private void StartSearch()
    {
        stateManager.SetState(EnemyStateManager.EnemyState.Search);
        isSearching = true;
        searchTimer = searchDuration;
        Debug.Log("🟠 Enemy is searching for the player...");
    }

    private void HandleSearchTimer()
    {
        searchTimer -= Time.deltaTime;
        if (searchTimer <= 0f)
        {
            EndSearch();
            stateManager.SetState(EnemyStateManager.EnemyState.Patrol);
        }
    }

    private void EndSearch()
    {
        isSearching = false;
        animator.ResetTrigger("punch");
        SetBlocking(false);
    }

    #endregion

    #region --- Utility Methods ---

    private void TransitionToChase()
    {
        EndSearch();
        stateManager.SetState(EnemyStateManager.EnemyState.Chase);
        agent.SetDestination(player.position);
        voiceManager?.PlayAlertedVoice();
    }

    private void RotateTowardsMovement()
    {
        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(agent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    private bool IsInCombatState()
    {
        var state = stateManager.currentState;
        return state == EnemyStateManager.EnemyState.Chase || state == EnemyStateManager.EnemyState.Attack;
    }

    private void ApplyGrudgeBehavior()
    {
        if (grudgeMemory == null) return;

        int grudge = grudgeMemory.GetGrudgeLevel();
        attackDistance += grudge * 0.5f;
        chaseDistance += grudge * 1f;
        agent.speed += grudge * 0.2f;

        Debug.Log($"[Grudge AI] Adjusted stats — Attack: {attackDistance}, Chase: {chaseDistance}");
    }

    private void ValidatePlayerHealthSystem()
    {
        var playerHealth = player.GetComponent<HealthSystem>();
        if (playerHealth != null)
            Debug.Log("✅ Found HealthSystem on player.");
        else
            Debug.LogError("❌ Player has no HealthSystem component!");
    }

    public void PlayHitAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("hit");
        }
        else
        {
            Debug.LogWarning("❌ Animator is null in PlayHitAnimation()");
        }
    }

    #endregion
}
