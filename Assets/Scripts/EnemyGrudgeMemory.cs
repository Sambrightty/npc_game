using UnityEngine;

/// <summary>
/// Tracks how many times the player has defeated this enemy,
/// increasing aggression across future encounters via a persistent "grudge level".
/// </summary>
public class EnemyGrudgeMemory : MonoBehaviour
{
    private const string GrudgeKey = "EnemyGrudgeLevel"; // Persistent PlayerPrefs key
    private int grudgeLevel;

    /// <summary>
    /// Public getter for the current grudge level (read-only).
    /// Used by external AI systems to adjust behavior.
    /// </summary>
    public int GrudgeLevel => grudgeLevel;

    /// <summary>
    /// Load the stored grudge level when this enemy is instantiated.
    /// </summary>
    private void Start()
    {
        grudgeLevel = PlayerPrefs.GetInt(GrudgeKey, 0);
        Debug.Log($"[Grudge Memory] Loaded Grudge Level: {grudgeLevel}");
    }

    /// <summary>
    /// Increase the grudge level by one and save it persistently.
    /// Call this when the player defeats the enemy.
    /// </summary>
    public void IncreaseGrudge()
    {
        grudgeLevel++;
        PlayerPrefs.SetInt(GrudgeKey, grudgeLevel);
        PlayerPrefs.Save();
        Debug.Log($"[Grudge Memory] Increased Grudge Level to: {grudgeLevel}");
    }

    /// <summary>
    /// Reset the grudge level to zero and remove it from storage.
    /// Useful for campaign resets or debugging.
    /// </summary>
    public void ResetGrudge()
    {
        grudgeLevel = 0;
        PlayerPrefs.DeleteKey(GrudgeKey);
        Debug.Log("[Grudge Memory] Grudge Level Reset");
    }

    /// <summary>
    /// Retrieve the current grudge level.
    /// </summary>
    public int GetGrudgeLevel()
    {
        return grudgeLevel;
    }
}
