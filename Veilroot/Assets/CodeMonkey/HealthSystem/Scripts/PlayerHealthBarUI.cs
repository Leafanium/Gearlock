using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using CodeMonkey.HealthSystemCM;
using UnityEngine.EventSystems;

public class PlayerHealthBarUI : MonoBehaviour
{
    [SerializeField] private Image barImage;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Text gameOverText;  // Standard Unity Text
    [SerializeField] private Button restartButton;
    [SerializeField] private AudioClip deathSound;
    private AudioSource audioSource;

    private HealthSystem healthSystem;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("[PlayerHealthBarUI] No GameObject tagged 'Player' found.");
            return;
        }

        if (HealthSystem.TryGetHealthSystem(player, out HealthSystem foundHealthSystem))
        {
            healthSystem = foundHealthSystem;
            healthSystem.OnHealthChanged += OnHealthChanged;
            UpdateHealthBar();
        }
        else
        {
            Debug.LogError("[PlayerHealthBarUI] Player does not have a HealthSystemComponent.");
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        gameOverPanel.SetActive(false);
        restartButton.onClick.AddListener(RestartGame);
    }

    private void OnHealthChanged(object sender, System.EventArgs e)
    {
        UpdateHealthBar();

        if (healthSystem.GetHealthNormalized() <= 0f)
        {
            TriggerGameOver();
        }
    }

    private void UpdateHealthBar()
    {
        if (barImage != null && healthSystem != null)
        {
            barImage.fillAmount = healthSystem.GetHealthNormalized();
        }
    }

    private void TriggerGameOver()
    {
        Debug.Log("Player has died. Game Over!");

        // Play death sound
        if (deathSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        // Display the Game Over screen
        Invoke(nameof(ShowGameOverScreen), 1f);
    }

    private void ShowGameOverScreen()
    {
        // Unlock the cursor for UI interaction
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Show the Game Over UI without pausing
        gameOverPanel.SetActive(true);
        gameOverText.text = "GAME OVER";
    }




    public void RestartGame()
    {
        Debug.Log("Restart button clicked!");

        // Reset time scale to normal
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }



    private void OnDestroy()
    {
        if (healthSystem != null)
        {
            healthSystem.OnHealthChanged -= OnHealthChanged;
        }
    }
}
