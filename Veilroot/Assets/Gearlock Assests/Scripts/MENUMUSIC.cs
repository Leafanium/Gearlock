using UnityEngine;

public class AudioTest : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.Play();
            Debug.Log("Playing audio via script.");
        }
        else
        {
            Debug.LogError("No AudioSource found on this GameObject.");
        }
    }
}
