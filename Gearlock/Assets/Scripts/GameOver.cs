using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverUI; // Assign a UI panel in the inspector

    private bool isGameOver = false;

    void Start()
    {
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false); // Hide game over screen at start
        }
    }

    public void TriggerGameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        Debug.Log("Game Over! Player has died.");

        // Show Game Over UI
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }

        // Stop time (optional)
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Reset time
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Restart current scene
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
