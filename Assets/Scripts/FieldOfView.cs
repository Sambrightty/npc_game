using UnityEngine;

/// <summary>
/// Defines awareness levels for an enemy based on visual or auditory stimuli.
/// </summary>
public enum AwarenessState
{
    Unaware,     // Default state — no player detected
    Suspicious,  // Heard a noise or partial visibility
    Alerted,     // Clearly sees the player at a distance
    Engaged      // Fully engaged, close proximity to the player
}

/// <summary>
/// Handles field of view detection and awareness logic for an enemy NPC.
/// </summary>
public class FieldOfView : MonoBehaviour
{
    [Header("Field of View Settings")]
    public float viewRadius = 10f;                      // Detection range
    [Range(0, 360)] public float viewAngle = 120f;      // Field of view angle

    [Header("Detection Layers")]
    public LayerMask playerMask;                        // Layer the player is on
    public LayerMask obstacleMask;                      // Layers that can obstruct view

    [Header("Target Settings")]
    public Transform target;                            // Reference to the player

    public bool CanSeePlayer { get; private set; }      // Flag to indicate if the player is visible

    private AwarenessState currentState = AwarenessState.Unaware;
    private Renderer rend;                              // Used to visually indicate awareness via color

    private void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend == null)
            Debug.LogWarning("Renderer missing on " + gameObject.name);
    }

    private void Update()
    {
        CheckPlayerInSight(); // Constantly check if the player is within view
    }

    /// <summary>
    /// Checks whether the player is in the field of view and unobstructed.
    /// Updates the CanSeePlayer flag and awareness state accordingly.
    /// </summary>
    private void CheckPlayerInSight()
    {
        if (target == null) return;

        Vector3 dirToPlayer = (target.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, target.position);

        // Check if player is within FOV angle
        if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2f)
        {
            // Check for obstacles using raycast
            if (!Physics.Raycast(transform.position, dirToPlayer, distanceToPlayer, obstacleMask))
            {
                // Player is clearly visible
                CanSeePlayer = true;
                Debug.DrawRay(transform.position, dirToPlayer * viewRadius, Color.green);

                // Set higher state depending on distance
                if (distanceToPlayer < 3f)
                    SetAwarenessState(AwarenessState.Engaged);
                else
                    SetAwarenessState(AwarenessState.Alerted);
            }
            else
            {
                // Player is blocked by an obstacle
                CanSeePlayer = false;
                Debug.DrawRay(transform.position, dirToPlayer * viewRadius, Color.red);
                SetAwarenessState(AwarenessState.Suspicious);
            }
        }
        else
        {
            // Player is outside the field of view angle
            CanSeePlayer = false;
            SetAwarenessState(AwarenessState.Unaware);
        }
    }

    /// <summary>
    /// Converts an angle to a direction vector (used for debugging or vision cone rendering).
    /// </summary>
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
            angleInDegrees += transform.eulerAngles.y;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    /// <summary>
    /// Updates the enemy's awareness state and changes color for visual feedback.
    /// </summary>
    public void SetAwarenessState(AwarenessState newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        Debug.Log("Awareness State changed to: " + newState);

        // Change material color to reflect state
        switch (currentState)
        {
            case AwarenessState.Unaware:
                rend.material.color = Color.gray;
                break;
            case AwarenessState.Suspicious:
                rend.material.color = Color.yellow;
                break;
            case AwarenessState.Alerted:
                rend.material.color = new Color(1f, 0.5f, 0f); // orange
                break;
            case AwarenessState.Engaged:
                rend.material.color = Color.red;
                break;
        }
    }

    /// <summary>
    /// Call this method to simulate the enemy hearing a suspicious sound.
    /// </summary>
    public void HearPlayer()
    {
        SetAwarenessState(AwarenessState.Suspicious);
    }
}
