using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void StartGame()
    {
        FindObjectOfType<FadeManager>().FadeToScene("Demo"); // Loads the game
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
