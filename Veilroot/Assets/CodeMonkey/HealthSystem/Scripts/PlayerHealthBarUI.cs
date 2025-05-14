using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.HealthSystemCM;

public class PlayerHealthBarUI : MonoBehaviour
{
    [SerializeField] private Image barImage;

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
    }

    private void OnHealthChanged(object sender, System.EventArgs e)
    {
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        if (barImage != null && healthSystem != null)
        {
            barImage.fillAmount = healthSystem.GetHealthNormalized();
        }
    }

    private void OnDestroy()
    {
        if (healthSystem != null)
        {
            healthSystem.OnHealthChanged -= OnHealthChanged;
        }
    }
}
