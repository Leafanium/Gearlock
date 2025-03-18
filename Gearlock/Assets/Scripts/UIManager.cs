using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Text healthText;
    public Text scoreText;
    public Text timerText;
    public GameObject gameOverPanel; // Game Over UI Panel

    private int score = 0;
    private float timer = 0f;
    private bool isTimerRunning = true;
    private PlayerCharacter player;

    void Start()
    {
        // Auto-assign UI elements if they are not assigned in Inspector
        if (healthText == null) healthText = GameObject.Find("Health").GetComponent<Text>();
        if (scoreText == null) scoreText = GameObject.Find("Score").GetComponent<Text>();
        if (timerText == null) timerText = GameObject.Find("Timer").GetComponent<Text>();

        // Find GameOverPanel
        if (gameOverPanel == null)
        {
            gameOverPanel = GameObject.Find("GameOverPanel");
            if (gameOverPanel == null)
            {
                Debug.LogError("GameOverPanel not found in the scene! Make sure it's inside the Canvas.");
            }
            else
            {
                gameOverPanel.SetActive(false); // Hide at start
            }
        }

        player = FindObjectOfType<PlayerCharacter>();
        if (player != null) UpdateHealthUI(player.GetCurrentHealth());

        UpdateScoreUI();
        StartCoroutine(UpdateTimer());
    }

    void Update()
    {
        if (isTimerRunning)
        {
            timer += Time.deltaTime;
            UpdateTimerUI();
        }
    }

    public void UpdateHealthUI(int currentHealth)
    {
        if (healthText != null) healthText.text = "Health: " + currentHealth;

        if (currentHealth <= 0) ShowGameOver();
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null) scoreText.text = "Score: " + score;
    }

    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);
        if (timerText != null) timerText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
    }

    public void StopTimer()
    {
        isTimerRunning = false;
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true); // Show Game Over overlay
            isTimerRunning = false; // Stop the timer
            DisablePlayerControls(); // Disable player movement and actions
        }
    }

    private void DisablePlayerControls()
    {
        if (player != null)
        {
            player.enabled = false; // Disables PlayerCharacter script
            player.GetComponent<Rigidbody>().velocity = Vector3.zero; // Stops movement
        }

        // Disable shooting if applicable
        RayShooter shooter = FindObjectOfType<RayShooter>();
        if (shooter != null) shooter.enabled = false;
    }
}
