using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Input Keys")]
    public KeyCode punchKey = KeyCode.Space;
    public KeyCode blockKey = KeyCode.B;
    public KeyCode healKey = KeyCode.H;

    [Header("Combat Settings")]
    public float punchRange = 2f;
    public float punchDamage = 10f;
    public Transform punchOrigin;
    public LayerMask enemyLayer;

    [Header("Healing Settings")]
    public float healRate = 10f;
    public float healCooldown = 5f;

    private float lastCombatTime = -10f;
    private int punchCount = 0;

    private bool isHealing = false;
    private bool isBlocking = false;

    private Rigidbody rb;
    private HealthSystem healthSystem;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        healthSystem = GetComponent<HealthSystem>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Move();
        HandlePunchInput();
        HandleBlockInput();
        HandleHealInput();
    }

    /// <summary>
    /// Handles player movement using WASD/Arrow keys.
    /// Also sets walking animation.
    /// </summary>
   void Move()
{
    float h = Input.GetAxis("Horizontal");
    float v = Input.GetAxis("Vertical");
    Vector3 inputDirection = new Vector3(h, 0, v).normalized;

    // Update walking animation state
    bool isWalking = inputDirection.magnitude >= 0.1f;
    animator.SetBool("isWalking", isWalking);

    if (isWalking)
    {
        // Rotate the player towards the movement direction
        Quaternion toRotation = Quaternion.LookRotation(inputDirection, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);

        // Move the player in the direction they are facing
        Vector3 moveDir = transform.forward * moveSpeed * Time.deltaTime;
        rb.MovePosition(transform.position + moveDir);
    }
}

    

    /// <summary>
    /// Checks for punch input and triggers attack logic and animation.
    /// </summary>
    void HandlePunchInput()
    {
        if (Input.GetKeyDown(punchKey))
        {
            animator.SetTrigger("isPunching");
            Punch();
            lastCombatTime = Time.time;
        }
    }

    /// <summary>
    /// Checks for block key press and toggles block state accordingly.
    /// </summary>
    void HandleBlockInput()
    {
        if (Input.GetKeyDown(blockKey))
        {
            SetBlockingState(true);
            lastCombatTime = Time.time;
            Debug.Log("🛡️ Player is blocking.");
        }

        if (Input.GetKeyUp(blockKey))
        {
            SetBlockingState(false);
            Debug.Log("🚫 Player stopped blocking.");
        }
    }

    /// <summary>
    /// Toggles the player's block state and informs the HealthSystem.
    /// </summary>
    void SetBlockingState(bool state)
    {
        isBlocking = state;
        animator.SetBool("isBlocking", state); // Animation updated
        if (healthSystem != null)
            healthSystem.IsBlocking = state;
    }

    /// <summary>
    /// Handles healing input and cooldown logic.
    /// </summary>
    void HandleHealInput()
    {
        if (Input.GetKey(healKey) && Time.time - lastCombatTime > healCooldown && !isHealing)
        {
            StartCoroutine(PlayerHealOverTime());
        }
    }

    /// <summary>
    /// Casts a punch ray forward to hit enemies within range.
    /// Applies damage if enemy is not blocking.
    /// </summary>
    void Punch()
    {
        Vector3 origin = punchOrigin.position;
        Vector3 direction = transform.forward;

        Debug.DrawRay(origin, direction * punchRange, Color.red, 1f);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, punchRange, enemyLayer))
        {
            Debug.Log("👊 Punch hit: " + hit.collider.name);
            HealthSystem enemyHealth = hit.collider.GetComponentInParent<HealthSystem>();

            if (enemyHealth != null)
            {
                if (!enemyHealth.IsBlocking)
                {
                    enemyHealth.TakeDamage(punchDamage);
                    punchCount++;
                    Debug.Log("✅ Enemy hit! Health reduced.");
                }
                else
                {
                    Debug.Log("🛡️ Enemy blocked the punch!");
                }
            }
            else
            {
                Debug.LogWarning("Hit object has no HealthSystem: " + hit.collider.name);
            }
        }
        else
        {
            Debug.Log("❌ Punch missed.");
        }
    }

    /// <summary>
    /// Returns the current blocking state of the player.
    /// </summary>
    public bool IsBlocking()
    {
        return isBlocking;
    }

    /// <summary>
    /// Coroutine to heal the player gradually while healing key is held.
    /// </summary>
    private IEnumerator PlayerHealOverTime()
    {
        isHealing = true;

        while (Input.GetKey(healKey) && healthSystem.currentHealth < healthSystem.maxHealth)
        {
            healthSystem.Heal(healRate * Time.deltaTime);
            yield return null;
        }

        isHealing = false;
    }

    /// <summary>
    /// Optional: Trigger hit reaction externally
    /// </summary>
    public void PlayHitAnimation()
    {
        animator.SetTrigger("isHit");
    }

    
}
