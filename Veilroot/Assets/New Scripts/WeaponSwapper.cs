using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponSwitcher : MonoBehaviour
{
    [Header("Prefab References")]
    public GameObject shotgunPrefab;
    public GameObject smgPrefab;

    [Header("Spawn Location")]
    public Transform weaponHolder;

    [Header("UI Elements")]
    public Image weaponIcon;
    public Sprite shotgunSprite;
    public Sprite smgSprite;
    public TMP_Text currentAmmoText;
    public TMP_Text reserveAmmoText;

    [Header("Ammo Settings")]
    public int smgMaxClip = 50;
    public int smgReserveMax = 150;
    public int shotgunMaxClip = 2;
    public int shotgunReserveMax = 10;

    [Header("Fire Rates")]
    public float smgFireRate = 0.1f;
    public float shotgunFireRate = 1.0f;

    [Header("Damage")]
    public float smgDamage = 10f;
    public float shotgunPelletDamage = 25f;

    [Header("Firing Mode")]
    public bool smgAuto = true;
    public bool shotgunAuto = false;

    [Header("Raycast Settings")]
    public LayerMask hitMask;

    [Header("FX")]
    public GameObject muzzleFlashPrefab;
    public Transform muzzleFlashPoint;
    public AudioSource shootAudio;
    public Camera mainCam;
    public float cameraShakeIntensity = 0.1f;
    public float cameraShakeDuration = 0.1f;
    public Animator weaponAnimator;
    public string fireTriggerName = "Fire";

    private GameObject currentWeapon;
    private int currentWeaponIndex = -1;
    private int currentAmmo;
    private int reserveAmmo;
    private float fireCooldown = 0f;

    void Start()
    {
        if (mainCam == null)
            mainCam = Camera.main;

        EquipWeapon(0);
    }

    void Update()
    {
        fireCooldown -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Alpha1)) EquipWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) EquipWeapon(1);

        bool isAuto = currentWeaponIndex == 0 ? shotgunAuto : smgAuto;

        if (((isAuto && Input.GetMouseButton(0)) || (!isAuto && Input.GetMouseButtonDown(0))) && fireCooldown <= 0f)
        {
            FireWeapon();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }

    void EquipWeapon(int index)
    {
        if (currentWeaponIndex == index && currentWeapon != null)
            return;

        currentWeaponIndex = index;

        if (currentWeapon != null)
            Destroy(currentWeapon);

        GameObject prefabToSpawn = index == 0 ? shotgunPrefab : smgPrefab;
        currentWeapon = Instantiate(prefabToSpawn, weaponHolder);
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;
        currentWeapon.transform.localScale = Vector3.one;

        weaponIcon.sprite = index == 0 ? shotgunSprite : smgSprite;

        currentAmmo = index == 0 ? shotgunMaxClip : smgMaxClip;
        reserveAmmo = index == 0 ? shotgunReserveMax : smgReserveMax;

        UpdateAmmoUI();
    }

    void FireWeapon()
    {
        if (currentAmmo <= 0) return;

        fireCooldown = currentWeaponIndex == 0 ? shotgunFireRate : smgFireRate;
        currentAmmo--;

        TriggerMuzzleFX();
        PlayShootAudio();
        TriggerRecoil();
        ShakeCamera();

        if (currentWeaponIndex == 0)
            FireShotgun();
        else
            FireSMG();

        UpdateAmmoUI();
    }

    void FireSMG()
    {
        if (mainCam == null) return;

        Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, hitMask) && hit.collider != null)
        {
            var enemy = hit.collider.GetComponentInParent<EnemyDetector>();
            if (enemy != null && enemy.gameObject.activeInHierarchy)
            {
                enemy.OnHit(smgDamage);
            }
        }
    }

    void FireShotgun()
    {
        if (mainCam == null) return;

        int pelletCount = 8;
        float spreadAngle = 5f;

        for (int i = 0; i < pelletCount; i++)
        {
            Vector3 direction = mainCam.transform.forward;
            direction += new Vector3(
                Random.Range(-spreadAngle, spreadAngle),
                Random.Range(-spreadAngle, spreadAngle),
                0
            ) * 0.01f;

            Ray ray = new Ray(mainCam.transform.position, direction);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, hitMask) && hit.collider != null)
            {
                var enemy = hit.collider.GetComponentInParent<EnemyDetector>();
                if (enemy != null && enemy.gameObject.activeInHierarchy)
                {
                    enemy.OnHit(shotgunPelletDamage);
                }
            }
        }
    }

    void Reload()
    {
        int maxClip = currentWeaponIndex == 0 ? shotgunMaxClip : smgMaxClip;
        int neededAmmo = maxClip - currentAmmo;

        if (reserveAmmo > 0 && neededAmmo > 0)
        {
            int ammoToLoad = Mathf.Min(neededAmmo, reserveAmmo);
            currentAmmo += ammoToLoad;
            reserveAmmo -= ammoToLoad;
            UpdateAmmoUI();
        }
    }

    void UpdateAmmoUI()
    {
        if (currentAmmoText != null)
            currentAmmoText.text = currentAmmo.ToString();

        if (reserveAmmoText != null)
            reserveAmmoText.text = reserveAmmo.ToString();
    }

    void TriggerMuzzleFX()
    {
        if (muzzleFlashPrefab != null && muzzleFlashPoint != null)
        {
            GameObject flash = Instantiate(muzzleFlashPrefab, muzzleFlashPoint.position, muzzleFlashPoint.rotation);
            Destroy(flash, 0.1f);
        }
    }

    void PlayShootAudio()
    {
        if (shootAudio != null)
        {
            shootAudio.Play();
        }
    }

    void TriggerRecoil()
    {
        if (weaponAnimator != null && !string.IsNullOrEmpty(fireTriggerName))
        {
            weaponAnimator.SetTrigger(fireTriggerName);
        }
    }

    void ShakeCamera()
    {
        if (mainCam == null) return;
        StartCoroutine(DoCameraShake());
    }

    System.Collections.IEnumerator DoCameraShake()
    {
        Vector3 originalPos = mainCam.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < cameraShakeDuration)
        {
            Vector3 randomOffset = Random.insideUnitSphere * cameraShakeIntensity;
            mainCam.transform.localPosition = originalPos + randomOffset;
            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCam.transform.localPosition = originalPos;
    }

    public void PickupAmmo(AmmoPickup.WeaponType type, int amount)
    {
        if (type == AmmoPickup.WeaponType.SMG)
        {
            smgReserveMax = Mathf.Min(smgReserveMax + amount, 150);
            if (currentWeaponIndex == 1)
            {
                reserveAmmo = Mathf.Min(reserveAmmo + amount, 150);
                UpdateAmmoUI();
            }
        }
        else
        {
            shotgunReserveMax = Mathf.Min(shotgunReserveMax + amount, 10);
            if (currentWeaponIndex == 0)
            {
                reserveAmmo = Mathf.Min(reserveAmmo + amount, 10);
                UpdateAmmoUI();
            }
        }
    }
}
