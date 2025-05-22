using UnityEngine;

/// <summary>
/// Simulates basic auditory perception for the enemy.
/// If the player enters the hearing zone and is making noise,
/// the enemy becomes suspicious via the FieldOfView system.
/// </summary>
public class EnemyHearing : MonoBehaviour
{
    [Tooltip("FieldOfView component responsible for awareness handling.")]
    [SerializeField] private FieldOfView fovScript;

    /// <summary>
    /// Triggered when a collider enters the enemy's hearing range.
    /// If it's the player and they are making noise, the enemy reacts.
    /// </summary>
    /// <param name="other">The collider entering the hearing trigger.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (IsLoudPlayer(other))
        {
            Debug.Log("👂 Sound detected! Enemy is now suspicious.");
            fovScript?.HearPlayer(); // Notify FOV script to increase awareness
        }
    }

    /// <summary>
    /// Determines if the collider is a noisy player.
    /// In the future, replace with real conditions (e.g., sprinting).
    /// </summary>
    /// <param name="collider">The collider to check.</param>
    /// <returns>True if the collider is a loud player.</returns>
    private bool IsLoudPlayer(Collider collider)
    {
        if (!collider.CompareTag("Player")) return false;

        // TODO: Add checks for actual noise-producing actions (e.g., sprinting, jumping)
        return true; // Stub: Assume player is always loud
    }
}
