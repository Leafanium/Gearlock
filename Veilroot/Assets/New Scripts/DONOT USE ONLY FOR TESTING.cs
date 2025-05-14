using CodeMonkey.HealthSystemCM;
using UnityEngine;

public class DebugDamage : MonoBehaviour
{
    private HealthSystem healthSystem;

    private void Start()
    {
        if (HealthSystem.TryGetHealthSystem(gameObject, out HealthSystem foundHealthSystem))
        {
            healthSystem = foundHealthSystem;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K) && healthSystem != null)
        {
            healthSystem.Damage(10f);
            Debug.Log("Manual damage applied for testing!");
        }
    }
}
            