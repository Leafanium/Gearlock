using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class StartupTransition : MonoBehaviour
{
    private VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.loopPointReached += OnVideoEnd; // Calls function when video ends
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        SceneManager.LoadScene("MainMenu"); // Loads main menu after video
    }
}
