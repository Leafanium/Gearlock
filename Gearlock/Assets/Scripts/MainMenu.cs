using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Demo"); // Loads the gameplay scene
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
