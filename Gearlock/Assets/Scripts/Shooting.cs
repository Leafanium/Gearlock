using System.Collections;
using UnityEngine;
using TMPro;

public class FPS_Shooting : MonoBehaviour
{
    [Header("Gun Settings")]
    public float damage = 20f;
    public float range = 100f;
    public float fireRate = 0.2f;
    public int maxAmmo = 30;
    private int currentAmmo;
    public float reloadTime = 1.5f;
    private bool isReloading = false;

    [Header("References")]
    public Camera fpsCam;
    public TMP_Text ammoText;
    public Animator gunAnimator;

    [Header("Audio")]
    public AudioSource gunAudioSource;
    public AudioClip gunShotClip;
    public AudioClip reloadClip;
    public AudioClip emptyClip;
    public AudioClip bulletImpactClip; // New impact sound

    private float nextTimeToFire = 0f;

    void Start()
    {
        currentAmmo = maxAmmo;
        UpdateAmmoUI();
    }

    void Update()
    {
        if (isReloading) return;

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + fireRate;
            Shoot();
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");

        // Play Reload Sound
        if (gunAudioSource && reloadClip)
            gunAudioSource.PlayOneShot(reloadClip);

        // Trigger reload animation if assigned
        if (gunAnimator)
            gunAnimator.SetTrigger("Reload");

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;
        UpdateAmmoUI();
    }

    void Shoot()
    {
        if (currentAmmo <= 0)
        {
            if (gunAudioSource && emptyClip)
                gunAudioSource.PlayOneShot(emptyClip);
            return;
        }

        currentAmmo--;
        UpdateAmmoUI();

        // Play Gunshot Sound
        if (gunAudioSource && gunShotClip)
            gunAudioSource.PlayOneShot(gunShotClip);

        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);

            // Play bullet impact sound if something is hit
            if (gunAudioSource && bulletImpactClip)
                gunAudioSource.PlayOneShot(bulletImpactClip);

            // Damage enemy if hit
            if (hit.transform.TryGetComponent<EnemyHealth>(out var target))
            {
                target.TakeDamage(damage);
            }
        }
    }

    void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = $"Ammo: {currentAmmo}/{maxAmmo}";
        }
    }
}
