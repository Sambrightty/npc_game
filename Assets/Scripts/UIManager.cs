using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Manages all game UI panels and user interactions such as pause, instructions,
/// quitting, and endgame results. Also coordinates enemy evolution via behavior tracking.
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject instructionsPanel;
    public GameObject pausePanel;
    public GameObject gameButtonsPanel;
    public GameObject quitConfirmationPanel;
    public GameObject endGamePanel;

    [Header("Result Display")]
    public TextMeshProUGUI resultText;

    [Header("References")]
    public PlayerBehaviorTracker playerBehaviorTracker;
    public GameObject enemyGameObject;

    private bool isPaused = false;
    private bool inInstructionOverlay = false;
    private bool gameStarted = false;

    private void Start()
    {
        Time.timeScale = 0f; // Pause the game at the start
        ShowMainMenu();
    }

    /// <summary>
    /// Starts the actual gameplay.
    /// </summary>
    public void StartGame()
    {
        gameStarted = true;
        Time.timeScale = 1f;

        mainMenuPanel.SetActive(false);
        instructionsPanel.SetActive(false);
        quitConfirmationPanel.SetActive(false);
        pausePanel.SetActive(false);
        gameButtonsPanel.SetActive(true);
    }

    /// <summary>
    /// Toggles the pause panel and pauses/resumes game time.
    /// </summary>
    public void TogglePause()
    {
        if (inInstructionOverlay || !gameStarted) return;

        isPaused = !isPaused;
        pausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    /// <summary>
    /// Displays the instruction panel and pauses the game.
    /// </summary>
    public void ShowInstructions()
    {
        inInstructionOverlay = true;
        Time.timeScale = 0f;

        instructionsPanel.SetActive(true);
        gameButtonsPanel.SetActive(false);
        pausePanel.SetActive(false);
        quitConfirmationPanel.SetActive(false);
    }

    /// <summary>
    /// Returns to the main menu or resumes the game based on state.
    /// </summary>
    public void BackToMenu()
    {
        instructionsPanel.SetActive(false);
        inInstructionOverlay = false;

        if (gameStarted)
        {
            gameButtonsPanel.SetActive(true);
            Time.timeScale = 1f;
        }
        else
        {
            ShowMainMenu();
        }
    }

    /// <summary>
    /// Restarts the current scene/game from scratch.
    /// </summary>
    public void RestartGame()
    {
        if (inInstructionOverlay) return;

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Displays the quit confirmation panel.
    /// </summary>
    public void QuitGame()
    {
        if (inInstructionOverlay || !gameStarted) return;

        quitConfirmationPanel.SetActive(true);
        gameButtonsPanel.SetActive(false);
        pausePanel.SetActive(false);

        Time.timeScale = 0f;
    }

    /// <summary>
    /// Confirms quitting to the main menu by restarting the scene.
    /// </summary>
    public void ConfirmQuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Cancels the quit process and resumes gameplay.
    /// </summary>
    public void CancelQuit()
    {
        quitConfirmationPanel.SetActive(false);
        gameButtonsPanel.SetActive(true);
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Displays the main menu and hides all other panels.
    /// </summary>
    private void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        instructionsPanel.SetActive(false);
        pausePanel.SetActive(false);
        quitConfirmationPanel.SetActive(false);
        gameButtonsPanel.SetActive(false);
    }

    /// <summary>
    /// Ends the game, shows result, and triggers AI evolution if the enemy wins.
    /// </summary>
    /// <param name="winner">The winning party ("Player" or "Enemy").</param>
    public void EndGame(string winner)
    {
        Time.timeScale = 0f;

        endGamePanel.SetActive(true);
        gameButtonsPanel.SetActive(false);
        pausePanel.SetActive(false);
        quitConfirmationPanel.SetActive(false);

        resultText.text = $"{winner} Wins!";

        if (winner == "Enemy")
        {
            TryEvolveEnemyAI();
        }

        if (playerBehaviorTracker != null)
        {
            playerBehaviorTracker.ResetBehavior();
        }
    }

    /// <summary>
    /// Attempts to evolve enemy AI based on the tracked player behavior.
    /// </summary>
    private void TryEvolveEnemyAI()
    {
        if (playerBehaviorTracker == null || enemyGameObject == null) return;

        string playerBehavior = playerBehaviorTracker.GetBehaviorType();
        GeneticEnemyAI enemyAI = enemyGameObject.GetComponent<GeneticEnemyAI>();

        if (enemyAI != null)
        {
            enemyAI.EvolveGene(playerBehavior);
            Debug.Log($"🧠 Enemy AI evolved based on player behavior: {playerBehavior}");
        }
        else
        {
            Debug.LogWarning("Enemy GameObject does not have a GeneticEnemyAI component.");
        }
    }
}
