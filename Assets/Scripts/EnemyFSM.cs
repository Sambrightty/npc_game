using UnityEngine;
using UnityEngine.AI;
using System.Collections;

/// <summary>
/// EnemyFSM handles the finite state machine logic for an enemy NPC,
/// transitioning between Patrol, Chase, Attack, Retreat, and Search states
/// based on player visibility, distance, and health conditions. Includes
/// adaptive behavior via GeneticEnemyAI and player tracking.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(EnemyStateManager))]
public class EnemyFSM : MonoBehaviour
{
    [Header("References")]
    public Transform player;                          // Target player object
    public FieldOfView fieldOfView;                   // Handles player visibility checks
    public HealthSystem healthSystem;                 // Enemy health and blocking logic
    public PlayerBehaviorTracker behaviorTracker;     // Tracks player behavior for AI evolution

    private NavMeshAgent agent;                       // Unity navigation agent
    private EnemyStateManager stateManager;           // Manages enemy's current FSM state
    private GeneticEnemyAI enemyAI;                   // Provides adaptive decision-making logic
    private EnemyGrudgeMemory grudgeMemory;           // Modifies stats based on grudge level
    private VoiceManager voiceManager;                // Handles enemy voice playback

    [Header("State Thresholds")]
    public float chaseDistance = 10f;                 // Distance within which enemy will chase
    public float attackDistance = 2f;                 // Distance within which enemy will attack
    public float lowHealthThreshold = 30f;            // Below this, enemy will retreat

    [Header("Search Settings")]
    public float searchDuration = 5f;                 // Time spent searching after losing player

    [Header("Retreat Settings")]
    public float healingRate = 5f;                    // Healing rate while retreating
    public float safeDistance = 8f;                   // Safe distance from player to heal

    private float attackCooldown = 1.5f;              // Delay between attacks
    private float attackTimer;
    private float searchTimer;

    private bool isSearching = false;
    private bool isRetreating = false;
    private bool isHealing = false;
    private Vector3 retreatTarget;

    private Animator animator;


    /// <summary>
    /// Initializes components and adjusts behavior based on grudge level.
    /// </summary>
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        stateManager = GetComponent<EnemyStateManager>();
        grudgeMemory = GetComponent<EnemyGrudgeMemory>();
        enemyAI = GetComponent<GeneticEnemyAI>();
        voiceManager = GetComponent<VoiceManager>();
        animator = GetComponentInChildren<Animator>();


        ApplyGrudgeBehavior(); // Boost aggression if enemy holds a grudge
    }

    /// <summary>
    /// FSM logic run every frame to determine enemy behavior.
    /// </summary>
    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool canSeePlayer = fieldOfView.CanSeePlayer;

        // Prioritize retreating if health is low
        if (healthSystem.currentHealth < lowHealthThreshold)
        {
            stateManager.SetState(EnemyStateManager.EnemyState.Retreat);
            Retreat();
        }
        // Attack if within striking distance and player is visible
        else if (canSeePlayer && distanceToPlayer <= attackDistance)
        {
            EndSearch();
            stateManager.SetState(EnemyStateManager.EnemyState.Attack);
            Attack();
        }
        // Chase if player is in view but far from attack range
        else if (canSeePlayer && distanceToPlayer <= chaseDistance)
        {
            EndSearch();
            stateManager.SetState(EnemyStateManager.EnemyState.Chase);
            Chase();
        }
        // Player was visible but now lost — begin searching
        else if (!canSeePlayer && IsInCombatState())
        {
            StartSearch();
        }
        // Searching logic countdown
        else if (isSearching)
        {
            searchTimer -= Time.deltaTime;
            if (searchTimer <= 0f)
            {
                EndSearch();
                stateManager.SetState(EnemyStateManager.EnemyState.Patrol);
            }
        }
    }

    /// <summary>
    /// Adjusts enemy aggression based on grudge level memory.
    /// </summary>
    private void ApplyGrudgeBehavior()
    {
        if (grudgeMemory == null) return;

        int grudge = grudgeMemory.GetGrudgeLevel();
        attackDistance += grudge * 0.5f;
        chaseDistance += grudge * 1f;
        agent.speed += grudge * 0.2f;

        Debug.Log($"[Grudge AI] Adjusted stats — Attack: {attackDistance}, Chase: {chaseDistance}");
    }

    /// <summary>
    /// Moves toward the player while playing a voice alert.
    /// </summary>
    private void Chase()
{
    if (player == null) return;

    agent.SetDestination(player.position);
    voiceManager?.PlayAlertedVoice();

    // Tell animator to play walking animation
    animator.SetBool("isWalking", true);
}


    /// <summary>
    /// Executes an attack cycle with cooldown and AI-driven decision-making.
    /// </summary>
  private void Attack()
{
    agent.SetDestination(transform.position); // Stop movement
    animator.SetBool("isWalking", false); // Stop walking animation

    attackTimer += Time.deltaTime;
    if (attackTimer < attackCooldown) return;
    attackTimer = 0f;

    string action = enemyAI.DecideNextAction();
    Debug.Log($"[AI Decision] Action: {action}");

    switch (action)
    {
        case "Attack":
            TryAttackPlayer();
            break;
        case "Block":
            StartBlock();
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


    /// <summary>
    /// Attempts to apply damage to the player if within range and unblocked.
    /// </summary>
 private void TryAttackPlayer()
{
    if (Vector3.Distance(transform.position, player.position) > attackDistance) return;

    HealthSystem playerHealth = player.GetComponent<HealthSystem>();
    if (playerHealth == null) return;

    if (!playerHealth.IsBlocking)
    {
        animator.SetTrigger("punch");  // Trigger punch animation
        playerHealth.TakeDamage(10f);
        Debug.Log("👊 Enemy punches the player!");
    }
    else
    {
        Debug.Log("🛡️ Player blocked the punch!");
    }
}


    private void StartBlock()
{
    healthSystem.IsBlocking = true;
    animator.SetBool("isBlocking", true);  // Start block animation
    Debug.Log("🛡️ Enemy blocks!");
    Invoke(nameof(StopBlocking), 1f); // End block after 1 second
}

private void StopBlocking()
{
    healthSystem.IsBlocking = false;
    animator.SetBool("isBlocking", false); // Stop block animation
}


    /// <summary>
    /// Moves the enemy to a safe location to heal.
    /// </summary>
   private void Retreat()
{
    if (!isRetreating)
    {
        Vector3 directionAway = (transform.position - player.position).normalized;
        retreatTarget = transform.position + directionAway * 10f;

        if (NavMesh.SamplePosition(retreatTarget, out NavMeshHit hit, 10f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            isRetreating = true;
            voiceManager?.PlayRetreatVoice();

            animator.SetBool("isWalking", true); // Start walking during retreat
        }
    }
    else
    {
        // Once at target location, begin healing
        if (Vector3.Distance(transform.position, retreatTarget) < 1.5f && !isHealing)
        {
            animator.SetBool("isWalking", false); // Stop walking when healing
            isHealing = true;
            StartCoroutine(HealOverTime());
        }
    }
}


    /// <summary>
    /// Heals the enemy over time while stationary.
    /// </summary>
    private IEnumerator HealOverTime()
    {
        Debug.Log("💊 Enemy starts healing...");

        while (healthSystem.currentHealth < healthSystem.maxHealth * 0.6f)
        {
            healthSystem.Heal(healingRate * Time.deltaTime);
            yield return null;
        }

        Debug.Log("✅ Enemy healed. Returning to patrol.");
        isHealing = false;
        isRetreating = false;
        stateManager.SetState(EnemyStateManager.EnemyState.Patrol);
    }

    /// <summary>
    /// Triggers search mode when the player is lost.
    /// </summary>
    private void StartSearch()
    {
        stateManager.SetState(EnemyStateManager.EnemyState.Search);
        isSearching = true;
        searchTimer = searchDuration;
        Debug.Log("🟠 Enemy is searching for the player...");
    }

    /// <summary>
    /// Ends the search mode.
    /// </summary>
    private void EndSearch()
    {
        isSearching = false;
    }

    /// <summary>
    /// Informs the adaptive AI about the player’s latest tactic.
    /// </summary>
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

    /// <summary>
    /// Returns true if enemy is currently in combat-related states.
    /// </summary>
    private bool IsInCombatState()
    {
        var state = stateManager.currentState;
        return state == EnemyStateManager.EnemyState.Chase || state == EnemyStateManager.EnemyState.Attack;
    }

        public void PlayHitAnimation()
{
    if (animator != null)
    {
        animator.SetTrigger("hit");
    }
}
}
