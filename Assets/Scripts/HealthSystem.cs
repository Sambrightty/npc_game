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
            lowHealthOverlay.enabled = false;
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

    currentHealth -= amount;
    currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

    Debug.Log($"{(isPlayer ? "Player" : "Enemy")} took damage: {amount}, health now: {currentHealth}");

    if (healthBar != null)
        healthBar.value = currentHealth;

    // ✅ Play hit animation
    if (isPlayer)
    {
        PlayerController controller = GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.PlayHitAnimation();
        }
    }
    else
    {
        EnemyFSM enemyFSM = GetComponent<EnemyFSM>();
        if (enemyFSM != null)
        {
            enemyFSM.PlayHitAnimation();
        }
    }

    HandleLowHealthEffects();

    if (currentHealth <= 0)
    {
        HandleDeath();
    }

    // Enemy voice logic
    if (!isPlayer && currentHealth <= maxHealth * 0.3f && !hasPlayedHurtVoice)
    {
        VoiceManager voiceManager = GetComponent<VoiceManager>();
        if (voiceManager != null)
        {
            voiceManager.PlayHurtVoice();
            hasPlayedHurtVoice = true;
        }
    }
}


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

        if (lowHealthOverlay != null)
            lowHealthOverlay.enabled = isLow;

        if (lowHealthAudio != null)
        {
            if (isLow && !hasPlayedLowHealthCue)
            {
                lowHealthAudio.Play();
                hasPlayedLowHealthCue = true;
            }
            else if (!isLow)
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
            PlayerController controller = GetComponent<PlayerController>();
            if (controller != null)
                controller.enabled = false;
        }
        else
        {
            EnemyFSM fsm = GetComponent<EnemyFSM>();
            if (fsm != null)
                fsm.enabled = false;
        }
    }
}
