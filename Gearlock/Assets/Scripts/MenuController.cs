using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Demo"); // Change to your actual game scene name
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
