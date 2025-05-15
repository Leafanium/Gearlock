using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Cleaned_SampleScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

//If you're reading this comment it means we've uploaded our assignment :D
