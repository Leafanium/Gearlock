using System.Collections; // Fixes IEnumerator error
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Text healthText;
    public Text scoreText;
    public Text timerText;

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

        player = FindObjectOfType<PlayerCharacter>();
        if (player != null) UpdateHealthUI(player.GetCurrentHealth());

        UpdateScoreUI();
        StartCoroutine(UpdateTimerCoroutine());
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

    private IEnumerator UpdateTimerCoroutine()
    {
        while (isTimerRunning)
        {
            UpdateTimerUI();
            yield return new WaitForSeconds(1f);
        }
    }
}
