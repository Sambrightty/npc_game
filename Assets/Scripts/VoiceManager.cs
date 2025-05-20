using UnityEngine;

/// <summary>
/// Manages enemy voice lines with cooldown to prevent audio spamming.
/// Plays specific voice clips based on enemy actions like alert, hurt, or retreat.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class VoiceManager : MonoBehaviour
{
    [Header("Voice Clips")]
    public AudioClip alertedClip;
    public AudioClip hurtClip;
    public AudioClip retreatClip;

    [Header("Settings")]
    [Tooltip("Cooldown time between voice clip plays to prevent audio overlap.")]
    [SerializeField] private float cooldown = 3f;

    private AudioSource audioSource;
    private float lastPlayTime = -5f;

    private void Start()
    {
        // Get the AudioSource component attached to this GameObject
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Plays a given voice clip if cooldown has passed.
    /// </summary>
    /// <param name="clip">The audio clip to be played.</param>
    public void PlayVoice(AudioClip clip)
    {
        if (clip == null || audioSource == null)
            return;

        if (Time.time - lastPlayTime >= cooldown)
        {
            audioSource.PlayOneShot(clip);
            lastPlayTime = Time.time;
        }
    }

    /// <summary>
    /// Plays the alert voice line (e.g., when the enemy detects the player).
    /// </summary>
    public void PlayAlertedVoice()
    {
        PlayVoice(alertedClip);
    }

    /// <summary>
    /// Plays the hurt voice line (e.g., when the enemy takes damage).
    /// </summary>
    public void PlayHurtVoice()
    {
        PlayVoice(hurtClip);
    }

    /// <summary>
    /// Plays the retreat voice line (e.g., when the enemy decides to back off).
    /// </summary>
    public void PlayRetreatVoice()
    {
        PlayVoice(retreatClip);
    }
}
