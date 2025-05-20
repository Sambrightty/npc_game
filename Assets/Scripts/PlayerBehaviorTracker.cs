using UnityEngine;
using TMPro;

public class PlayerBehaviorTracker : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI behaviorText;  // TMP reference

    [Header("Action Counters")]
    public int punchCount = 0;
    public int blockCount = 0;

    [Header("Cooldown Settings")]
    [SerializeField] private float punchCooldown = 1f;
    [SerializeField] private float blockCooldown = 1f;

    private float punchTimer = 0f;
    private float blockTimer = 0f;

    private void Update()
    {
        punchTimer += Time.deltaTime;
        blockTimer += Time.deltaTime;

        HandlePunchInput();
        HandleBlockInput();

        UpdateBehaviorDisplay(); // Update UI
    }

    private void HandlePunchInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && punchTimer > punchCooldown)
        {
            punchCount++;
            punchTimer = 0f;
            Debug.Log("Punch count: " + punchCount);
        }
    }

    private void HandleBlockInput()
    {
        if (Input.GetKeyDown(KeyCode.B) && blockTimer > blockCooldown)
        {
            blockCount++;
            blockTimer = 0f;
            Debug.Log("Block count: " + blockCount);
        }
    }

    public string GetBehaviorType()
    {
        if (punchCount > blockCount + 3)
            return "Aggressive";
        else if (blockCount > punchCount + 3)
            return "Defensive";
        else
            return "Balanced";
    }

    public void ResetBehavior()
    {
        punchCount = 0;
        blockCount = 0;
    }

    /// <summary>
    /// Updates the TextMeshPro behavior label in the UI.
    /// </summary>
    private void UpdateBehaviorDisplay()
    {
        if (behaviorText != null)
            behaviorText.text = "Behavior: " + GetBehaviorType();
    }
}
