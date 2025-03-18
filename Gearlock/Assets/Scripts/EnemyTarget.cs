using System.Collections;
using UnityEngine;

public class EnemyTarget : MonoBehaviour
{
    private UIManager uiManager;

    void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
    }

    public IEnumerator Die()
    {
        this.transform.Rotate(-75, 0, 0);
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }

    public void ReactToHit()
    {
        WanderingAI behavior = GetComponent<WanderingAI>();
        if (behavior != null)
        {
            behavior.SetAlive(false);
            StartCoroutine(Die());

            if (uiManager != null)
            {
                uiManager.AddScore(10); // Adds score per kill
            }
        }
    }
}
