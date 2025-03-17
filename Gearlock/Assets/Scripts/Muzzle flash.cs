using UnityEngine;

public class ShotgunFire : MonoBehaviour
{
    public ParticleSystem muzzleFlashParticles; // Assign in Inspector

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left-click to fire
        {
            if (muzzleFlashParticles != null)
                muzzleFlashParticles.Play(); // Trigger the muzzle flash effect
        }
    }
}
