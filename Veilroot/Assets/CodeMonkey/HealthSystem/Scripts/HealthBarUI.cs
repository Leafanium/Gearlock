using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.HealthSystemCM;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Image barImage;
    [SerializeField] private GameObject getHealthSystemGameObject;

    private HealthSystem healthSystem;

    private void Start()
    {
        if (getHealthSystemGameObject == null)
        {
            Debug.LogWarning($"[HealthBarUI] getHealthSystemGameObject not assigned on '{gameObject.name}'");
            return;
        }

        if (HealthSystem.TryGetHealthSystem(getHealthSystemGameObject, out HealthSystem foundHealthSystem))
        {
            SetHealthSystem(foundHealthSystem);
        }
        else
        {
            Debug.LogWarning($"[HealthBarUI] '{getHealthSystemGameObject.name}' does not implement IGetHealthSystem.");
        }
    }

    public void SetHealthSystem(HealthSystem healthSystem)
    {
        this.healthSystem = healthSystem;
        healthSystem.OnHealthChanged += HealthSystem_OnHealthChanged;
        UpdateHealthBar();
    }

    private void HealthSystem_OnHealthChanged(object sender, System.EventArgs e)
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
            healthSystem.OnHealthChanged -= HealthSystem_OnHealthChanged;
        }
    }
}
