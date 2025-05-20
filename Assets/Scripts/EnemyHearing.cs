using UnityEngine;

/// <summary>
/// EnemyHearing simulates basic auditory perception for the enemy.
/// It detects the player's presence via a trigger zone and notifies
/// the FieldOfView system if the player is making noise.
/// </summary>
public class EnemyHearing : MonoBehaviour
{
    [Tooltip("Reference to the FieldOfView script handling vision and awareness.")]
    public FieldOfView fovScript;

    /// <summary>
    /// Called when another collider enters the enemy's hearing trigger.
    /// If the collider is the player and is making a loud action,
    /// the enemy reacts by becoming suspicious.
    /// </summary>
    /// <param name="other">The collider entering the trigger.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // TODO: Replace PlayerIsLoud with real checks (e.g., sprinting, shooting)
            if (PlayerIsLoud(other))
            {
                Debug.Log("👂 Sound detected! Enemy is now suspicious.");
                fovScript.HearPlayer(); // Triggers a state change or awareness shift
            }
        }
    }

    /// <summary>
    /// Determines if the player is performing a loud action.
    /// Currently stubbed to always return true for testing purposes.
    /// </summary>
    /// <param name="player">The player collider.</param>
    /// <returns>True if the player is loud (e.g., sprinting); otherwise false.</returns>
    private bool PlayerIsLoud(Collider player)
    {
        // Placeholder for actual sound-based logic (e.g., velocity check, animation state)
        return true;
    }
}
