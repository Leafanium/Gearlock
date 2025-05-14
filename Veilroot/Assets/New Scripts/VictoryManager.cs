using UnityEngine;

public class VictoryManager : MonoBehaviour
{
    public static VictoryManager Instance;

    public GameObject winScreen; // Assign this in the inspector

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public static void CheckForWin()
    {
        if (EnemyTracker.activeEnemyCount <= 0 && Instance != null)
        {
            Instance.TriggerVictory();
        }
    }

    public void TriggerVictory()
    {
        Debug.Log("🎉 YOU WIN!");
        Time.timeScale = 0f;
        if (winScreen != null)
            winScreen.SetActive(true);
    }
}
