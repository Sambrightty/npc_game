using UnityEngine;

/// <summary>
/// Represents the enemy's attack hitbox that deals damage to the player.
/// Should be attached to a weapon or collider used during attack animations.
/// </summary>
public class EnemyHitbox : MonoBehaviour
{
    /// <summary>
    /// Triggered when another collider enters the hitbox.
    /// If the collider is the player, apply damage via their HealthSystem.
    /// </summary>
    /// <param name="other">The collider entering the trigger.</param>
    private void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Player"))
    {
        Debug.Log("👊 Enemy hit the player!");

        // Search for HealthSystem on this object or its parents
        HealthSystem playerHealth = other.GetComponentInParent<HealthSystem>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(10f);
        }
        else
        {
            Debug.LogWarning("⚠️ Could not find HealthSystem on Player!");
        }
    }
}

}
