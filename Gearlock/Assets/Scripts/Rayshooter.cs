using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayShooter : MonoBehaviour
{
    private Camera cam;
    public AudioClip[] shootingSounds; // Array for multiple shooting sounds (Assign 3 in Inspector)
    public AudioClip reloadSound;   // Assign reload sound (Optional)
    public AudioClip outOfAmmoSound; // Sound when out of ammo (Assign in Inspector)

    private AudioSource audioSource;

    public int maxAmmo = 6; // Maximum ammo capacity
    private int currentAmmo;
    public float reloadTime = 2f; // Time in seconds to reload
    private bool isReloading = false;

    void Start()
    {
        cam = GetComponent<Camera>();

        // Hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Get or add an AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        currentAmmo = maxAmmo; // Start with full ammo
    }

    private void OnGUI()
    {
        int size = 24;
        float posX = cam.pixelWidth / 2 - size / 4;
        float posY = cam.pixelHeight / 2 - size / 2;
        GUI.Label(new Rect(posX, posY, size, size), "+");

        // Display ammo count on screen
        GUI.Label(new Rect(10, 10, 200, 30), "Ammo: " + currentAmmo + " / " + maxAmmo);
    }

    private IEnumerator SphereIndicator(Vector3 pos)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = pos;
        yield return new WaitForSeconds(1);
        Destroy(sphere);
    }

    void Update()
    {
        // Check if reloading
        if (isReloading)
        {
            return;
        }

        // Reload if 'R' is pressed
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo)
        {
            StartCoroutine(Reload());
            return;
        }

        // When the player left-clicks, check ammo and shoot
        if (Input.GetMouseButtonDown(0))
        {
            if (currentAmmo > 0)
            {
                Shoot();
            }
            else
            {
                OutOfAmmo();
            }
        }
    }

    void Shoot()
    {
        // Play a random shooting sound
        if (audioSource != null && shootingSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, shootingSounds.Length);
            audioSource.PlayOneShot(shootingSounds[randomIndex]);
        }

        // Reduce ammo count
        currentAmmo--;

        // Calculate the center of the screen
        Vector3 point = new Vector3(cam.pixelWidth / 2, cam.pixelHeight / 2, 0);
        Ray ray = cam.ScreenPointToRay(point);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log("Hit: " + hit.point);

            GameObject hitObject = hit.transform.gameObject;
            EnemyTarget target = hitObject.GetComponent<EnemyTarget>();
            if (target != null)
            {
                target.ReactToHit();
                Debug.Log("Target hit!");
            }
            else
            {
                StartCoroutine(SphereIndicator(hit.point));
            }
        }

        Debug.Log("Ammo left: " + currentAmmo);
    }

    void OutOfAmmo()
    {
        Debug.Log("Out of ammo! Press 'R' to reload.");

        // Play out-of-ammo sound if assigned
        if (audioSource != null && outOfAmmoSound != null)
        {
            audioSource.PlayOneShot(outOfAmmoSound);
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");

        if (audioSource != null && reloadSound != null)
        {
            audioSource.PlayOneShot(reloadSound);
        }

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;
        Debug.Log("Reloaded!");
    }
}
