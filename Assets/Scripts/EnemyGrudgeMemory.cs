using UnityEngine;

/// <summary>
/// EnemyGrudgeMemory tracks how many times the player has defeated this enemy,
/// increasing aggression in future encounters by storing a "grudge level".
/// The value is persisted using PlayerPrefs across game sessions.
/// </summary>
public class EnemyGrudgeMemory : MonoBehaviour
{
    private string grudgeKey = "EnemyGrudgeLevel";    // Key used to store grudge in PlayerPrefs
    public int grudgeLevel = 0;                       // Current grudge level (modifiable by other systems)

    /// <summary>
    /// Loads the saved grudge level from PlayerPrefs when the enemy spawns.
    /// </summary>
    void Start()
    {
        grudgeLevel = PlayerPrefs.GetInt(grudgeKey, 0); // Default to 0 if no previous grudge saved
        Debug.Log($"[Grudge Memory] Loaded Grudge Level: {grudgeLevel}");
    }

    /// <summary>
    /// Increments the enemy's grudge level and saves it persistently.
    /// Called when the player defeats this enemy.
    /// </summary>
    public void IncreaseGrudge()
    {
        grudgeLevel++;
        PlayerPrefs.SetInt(grudgeKey, grudgeLevel);  // Save the updated level
        PlayerPrefs.Save();                          // Ensure it's written to disk
        Debug.Log($"[Grudge Memory] Increased Grudge Level to: {grudgeLevel}");
    }

    /// <summary>
    /// Resets the enemy's grudge level to 0 and clears it from PlayerPrefs.
    /// Useful for restarting campaigns or new players.
    /// </summary>
    public void ResetGrudge()
    {
        grudgeLevel = 0;
        PlayerPrefs.DeleteKey(grudgeKey);            // Remove from persistent storage
        Debug.Log("[Grudge Memory] Grudge Level Reset");
    }

    /// <summary>
    /// Returns the current grudge level.
    /// Used by AI systems to modify behavior.
    /// </summary>
    public int GetGrudgeLevel()
    {
        return grudgeLevel;
    }
}
