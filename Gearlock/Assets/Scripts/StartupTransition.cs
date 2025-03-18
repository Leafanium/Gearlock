using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class StartupTransition : MonoBehaviour
{
    public float fallbackTime = 5f; // Fallback in case video fails

    private VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.loopPointReached += OnVideoEnd; 
        Invoke("ForceLoadMenu", fallbackTime); // Fallback transition
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        CancelInvoke("ForceLoadMenu");
        SceneManager.LoadScene("Menu");
    }

    void ForceLoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
