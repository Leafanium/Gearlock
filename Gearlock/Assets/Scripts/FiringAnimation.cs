using UnityEngine;

public class ShotgunAnimation : MonoBehaviour
{
    public Animator animator; // Assign in Inspector

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Press left-click
        {
            animator.SetBool("IsFiring", true);
        }

        if (Input.GetMouseButtonUp(0)) // Release left-click
        {
            animator.SetBool("IsFiring", false);
        }
    }
}
