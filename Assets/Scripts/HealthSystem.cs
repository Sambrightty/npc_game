using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles health logic for player or enemy, including damage, healing,
/// health bar updates, low health warnings, and death events.
/// </summary>
public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    public Slider healthBar;
    public bool isPlayer = false;

    [Header("Combat Flags")]
    public bool IsBlocking { get; set; }

    private EnemyGrudgeMemory grudgeMemory;

    [Header("Low Health Feedback")]
    public Image lowHealthOverlay;
    public AudioSource lowHealthAudio;
    public float lowHealthThreshold = 30f;
    public float fadeSpeed = 1f;

    private Color transparentRed = new Color(1, 0, 0, 0f);
    private Color visibleRed = new Color(1, 0, 0, 0.3f);

    private bool hasPlayedLowHealthCue = false;
    private bool hasPlayedHurtVoice = false;
     private Animator animator;

    private void Start()
    {
        currentHealth = maxHealth;
        grudgeMemory = GetComponent<EnemyGrudgeMemory>();

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

        if (lowHealthOverlay != null)
        {
            lowHealthOverlay.color = transparentRed;
            lowHealthOverlay.enabled = true;
        }
    }

    private void Update()
    {
        HandleLowHealthOverlay();
    }

    /// <summary>
    /// Fades in/out the red overlay based on current health level.
    /// </summary>
    private void HandleLowHealthOverlay()
    {
        if (lowHealthOverlay == null) return;

        bool isLow = currentHealth <= lowHealthThreshold;
        Color targetColor = isLow ? visibleRed : transparentRed;
        lowHealthOverlay.color = Color.Lerp(lowHealthOverlay.color, targetColor, Time.deltaTime * fadeSpeed);
    }

    /// <summary>
    /// Applies damage, updates health, handles death, and low-health reactions.
    /// </summary>
 public void TakeDamage(float amount)
{
    if (IsBlocking)
    {
        Debug.Log($"{(isPlayer ? "Player" : "Enemy")} blocked the attack. No damage taken.");
        return;
    }

    // Apply damage
    currentHealth -= amount;
    currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

    Debug.Log($"{gameObject.name} took damage: {amount}, health now: {currentHealth}");

    // Update health bar
    if (healthBar != null)
        healthBar.value = currentHealth;

    // Trigger hit animation
    if (isPlayer)
    {
        Debug.Log("Player got hit – trying to trigger animation");
        var controller = GetComponent<PlayerController>();
        if (controller != null)
        {
            Debug.Log("✅ Found PlayerController");
            controller.PlayHitAnimation();
        }
        else
        {
            Debug.LogWarning("❌ PlayerController NOT found!");
        }
    }
    else
    {
        Debug.Log("Enemy got hit – trying to trigger animation");
        var enemyFSM = GetComponent<EnemyFSM>();
        if (enemyFSM != null)
        {
            Debug.Log("✅ Found EnemyFSM");
            enemyFSM.PlayHitAnimation();
        }
    }

    // Handle low health effects (UI/audio)
    HandleLowHealthEffects();

    // Check death
    if (currentHealth <= 0)
    {
        HandleDeath();
    }

    // Play enemy low-health voice cue once
    if (!isPlayer && currentHealth <= maxHealth * 0.3f && !hasPlayedHurtVoice)
    {
        var voiceManager = GetComponent<VoiceManager>();
        if (voiceManager != null)
        {
            voiceManager.PlayHurtVoice();
            hasPlayedHurtVoice = true;
        }
    }
}



// public void TakeDamage(float amount)
// {
//     currentHealth -= amount;
//     currentHealth = Mathf.Max(currentHealth, 0);

//     Debug.Log($"{gameObject.name} took damage: {amount}, health now: {currentHealth}");
// }


    /// <summary>
    /// Heals the character and updates visual/audio feedback.
    /// </summary>
    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (healthBar != null)
            healthBar.value = currentHealth;

        HandleLowHealthEffects();
    }

    /// <summary>
    /// Returns true if the character's health has reached zero.
    /// </summary>
    public bool IsDead()
    {
        return currentHealth <= 0;
    }

    /// <summary>
    /// Enables or disables health-related UI and audio effects.
    /// </summary>
    private void HandleLowHealthEffects()
    {
        bool isLow = currentHealth <= lowHealthThreshold;

        // Audio cue
        if (lowHealthAudio != null)
        {
            if (isLow && !hasPlayedLowHealthCue)
            {
                lowHealthAudio.Play();
                hasPlayedLowHealthCue = true;
            }
            else if (!isLow && lowHealthAudio.isPlaying)
            {
                lowHealthAudio.Stop();
                hasPlayedLowHealthCue = false;
            }
        }
    }

    /// <summary>
    /// Handles death logic, notifies UIManager, and disables movement.
    /// </summary>
    private void HandleDeath()
    {
        Debug.Log($"{(isPlayer ? "Player" : "Enemy")} is Dead!");

        UIManager ui = FindFirstObjectByType<UIManager>();
        if (ui != null)
        {
            ui.EndGame(isPlayer ? "Enemy" : "Player");
        }

        DisableMovement();
    }

    /// <summary>
    /// Disables character-specific movement scripts upon death.
    /// </summary>
    private void DisableMovement()
    {
        if (isPlayer)
        {
            var controller = GetComponent<PlayerController>();
            if (controller != null)
                controller.enabled = false;
        }
        else
        {
            var fsm = GetComponent<EnemyFSM>();
            if (fsm != null)
                fsm.enabled = false;
        }
    }
}
