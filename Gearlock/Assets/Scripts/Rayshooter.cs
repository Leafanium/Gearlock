using System.Collections;
using UnityEngine;

public class RayShooter : MonoBehaviour
{
    private Camera cam;
    public AudioClip[] shootingSounds;
    public AudioClip reloadSound;
    public AudioClip outOfAmmoSound;

    private AudioSource audioSource;
    public int maxAmmo = 6;
    private int currentAmmo;
    public float reloadTime = 2f;
    private bool isReloading = false;

    void Start()
    {
        cam = GetComponent<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        currentAmmo = maxAmmo;
    }

    private void OnGUI()
    {
        int size = 24;
        float posX = cam.pixelWidth / 2 - size / 4;
        float posY = cam.pixelHeight / 2 - size / 2;
        GUI.Label(new Rect(posX, posY, size, size), "+");
        GUI.Label(new Rect(10, 10, 200, 30), "Ammo: " + currentAmmo + " / " + maxAmmo);
    }

    void Update()
    {
        if (isReloading)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo)
        {
            StartCoroutine(Reload());
            return;
        }

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
        if (audioSource != null && shootingSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, shootingSounds.Length);
            audioSource.PlayOneShot(shootingSounds[randomIndex]);
        }

        currentAmmo--;

        Vector3 point = new Vector3(cam.pixelWidth / 2, cam.pixelHeight / 2, 0);
        Ray ray = cam.ScreenPointToRay(point);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitObject = hit.transform.gameObject;
            EnemyTarget target = hitObject.GetComponent<EnemyTarget>();
            if (target != null)
            {
                target.ReactToHit();
            }
        }
    }

    void OutOfAmmo()
    {
        if (audioSource != null && outOfAmmoSound != null)
        {
            audioSource.PlayOneShot(outOfAmmoSound);
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;

        if (audioSource != null && reloadSound != null)
        {
            audioSource.PlayOneShot(reloadSound);
        }

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;
    }
}
