using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class StartupTransition : MonoBehaviour
{
    public float fallbackTime = 5f; // Time in case video fails

    private VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.loopPointReached += OnVideoEnd; 
        Invoke("ForceLoadMenu", fallbackTime); // Backup transition
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        CancelInvoke("ForceLoadMenu"); // Cancel backup if video finishes
        SceneManager.LoadScene("Menu");
    }

    void ForceLoadMenu()
    {
        SceneManager.LoadScene("Menu"); // Loads menu if video doesn't play
    }
}
