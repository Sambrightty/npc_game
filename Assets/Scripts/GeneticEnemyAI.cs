using UnityEngine;
using TMPro;

/// <summary>
/// Controls enemy behavior traits using a simplified genetic algorithm.
/// Traits adapt based on mutation and player strategy feedback.
/// </summary>
public class GeneticEnemyAI : MonoBehaviour
{
    [Header("Enemy Genes (Traits)")]
    [Range(0f, 1f)] public float aggression = 0.5f;  // Likelihood of attacking
    [Range(0f, 1f)] public float defense = 0.5f;     // Likelihood of blocking
    [Range(0f, 1f)] public float dodge = 0.5f;       // Likelihood of dodging

    [Header("Genetic Algorithm Settings")]
    [Range(0f, 0.5f)] public float mutationRate = 0.1f;  // Probability of mutation per trait

    private const string saveKeyPrefix = "EnemyGene_";   // Key prefix for saving genes

    [Header("UI References (Optional)")]
    public TextMeshProUGUI aggressionText;
    public TextMeshProUGUI defenseText;
    public TextMeshProUGUI dodgeText;

    /// <summary>
    /// Loads genes from persistent storage when the game starts.
    /// </summary>
    private void Start()
    {
        LoadGenes();
    }

    /// <summary>
    /// Mutates each gene slightly based on mutationRate and clamps the values.
    /// </summary>
    public void MutateGenes()
    {
        aggression = MutateValue(aggression);
        defense = MutateValue(defense);
        dodge = MutateValue(dodge);

        SaveGenes();
        Debug.Log($"Genes mutated: Aggression={aggression:F2}, Defense={defense:F2}, Dodge={dodge:F2}");
    }

    /// <summary>
    /// Applies random mutation to a given gene value.
    /// </summary>
    private float MutateValue(float value)
    {
        if (Random.value < mutationRate)
        {
            float mutationAmount = Random.Range(-0.1f, 0.1f);
            value = Mathf.Clamp01(value + mutationAmount);
        }
        return value;
    }

    /// <summary>
    /// Saves current genes to PlayerPrefs for persistence.
    /// </summary>
    public void SaveGenes()
    {
        PlayerPrefs.SetFloat(saveKeyPrefix + "Aggression", aggression);
        PlayerPrefs.SetFloat(saveKeyPrefix + "Defense", defense);
        PlayerPrefs.SetFloat(saveKeyPrefix + "Dodge", dodge);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Loads gene values from PlayerPrefs or uses default values.
    /// </summary>
    public void LoadGenes()
    {
        aggression = PlayerPrefs.GetFloat(saveKeyPrefix + "Aggression", aggression);
        defense = PlayerPrefs.GetFloat(saveKeyPrefix + "Defense", defense);
        dodge = PlayerPrefs.GetFloat(saveKeyPrefix + "Dodge", dodge);
    }

    /// <summary>
    /// Makes a behavioral decision based on current gene probabilities.
    /// </summary>
    public string DecideNextAction()
    {
        float roll = Random.value;

        if (roll < aggression) return "Attack";
        if (roll < aggression + defense) return "Block";
        if (roll < aggression + defense + dodge) return "Dodge";

        return "Idle"; // Fallback
    }

    /// <summary>
    /// Adjusts genes based on observed player behavior patterns.
    /// </summary>
    public void EvolveGene(string behavior)
    {
        Debug.Log($"🧬 Evolving genes based on player behavior: {behavior}");

        switch (behavior)
        {
            case "Aggressive":
                defense = MutateValue(defense + 0.1f);
                dodge = MutateValue(dodge + 0.1f);
                break;

            case "Defensive":
                aggression = MutateValue(aggression + 0.1f);
                break;

            case "Balanced":
                MutateGenes();
                break;
        }

        SaveGenes();
    }

    /// <summary>
    /// Updates the UI text fields to reflect current gene values.
    /// </summary>
    private void Update()
    {
        if (aggressionText != null)
            aggressionText.text = $"Aggression: {aggression:F2}";
        if (defenseText != null)
            defenseText.text = $"Defense: {defense:F2}";
        if (dodgeText != null)
            dodgeText.text = $"Dodge: {dodge:F2}";
    }
}
