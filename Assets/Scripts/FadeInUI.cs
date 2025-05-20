using UnityEngine;

/// <summary>
/// Smoothly fades in a UI element when the GameObject is enabled.
/// Attach this to any UI object with a CanvasGroup component.
/// </summary>
public class FadeInUI : MonoBehaviour
{
    [Header("Fade Settings")]
    public CanvasGroup canvasGroup;     // CanvasGroup controls transparency
    public float fadeSpeed = 1f;        // Speed of fade (units per second)

    /// <summary>
    /// Called automatically when the object becomes active in the scene.
    /// Starts the fade-in process.
    /// </summary>
    private void OnEnable()
    {
        if (canvasGroup == null)
        {
            Debug.LogWarning("❗ CanvasGroup reference missing on " + gameObject.name);
            return;
        }

        canvasGroup.alpha = 0f;         // Start fully transparent
        StartCoroutine(FadeIn());
    }

    /// <summary>
    /// Coroutine to incrementally fade the UI in over time.
    /// </summary>
    private System.Collections.IEnumerator FadeIn()
    {
        while (canvasGroup.alpha < 1f)
        {
            // Use unscaled delta time to allow fade during pause
            canvasGroup.alpha += Time.unscaledDeltaTime * fadeSpeed;
            canvasGroup.alpha = Mathf.Clamp01(canvasGroup.alpha); // Ensure value stays within 0–1
            yield return null;
        }
    }
}
