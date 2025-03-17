using UnityEngine;

public class WeaponShoot : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>(); // Get the Animator component
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left-click
        {
            animator.SetTrigger("Fire"); // Trigger the shoot animation
        }
    }
}
