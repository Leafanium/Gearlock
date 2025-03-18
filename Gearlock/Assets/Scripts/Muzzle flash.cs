using UnityEngine;

public class ShotgunFire : MonoBehaviour
{
    public ParticleSystem muzzleFlashParticles;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (muzzleFlashParticles != null)
                muzzleFlashParticles.Play();
        }
    }
}
