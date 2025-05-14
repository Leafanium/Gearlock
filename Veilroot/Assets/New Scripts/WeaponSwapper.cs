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

    private GameObject currentWeapon;
    private int currentWeaponIndex = -1;
    private int currentAmmo;
    private int reserveAmmo;
    private float fireCooldown = 0f;

    void Start()
    {
        EquipWeapon(0); // Start with Shotgun
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
        if (currentAmmo <= 0)
        {
            Debug.Log("Click – No ammo.");
            return;
        }

        fireCooldown = currentWeaponIndex == 0 ? shotgunFireRate : smgFireRate;
        currentAmmo--;
        Debug.Log($"Fired weapon. Ammo now: {currentAmmo}/{reserveAmmo}");

        if (currentWeaponIndex == 0)
            FireShotgun();
        else
            FireSMG();

        UpdateAmmoUI();
    }

    void FireSMG()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.red, 1f);
            EnemyDetector enemy = hit.collider.GetComponent<EnemyDetector>();
            if (enemy != null)
            {
                enemy.OnHit(smgDamage);
            }
            else
            {
                Debug.Log($"SMG hit {hit.collider.name} (no EnemyDetector attached).");
            }
        }
    }

    void FireShotgun()
    {
        int pelletCount = 8;
        float spreadAngle = 5f;

        for (int i = 0; i < pelletCount; i++)
        {
            Vector3 direction = Camera.main.transform.forward;
            direction += new Vector3(
                Random.Range(-spreadAngle, spreadAngle),
                Random.Range(-spreadAngle, spreadAngle),
                0
            ) * 0.01f;

            Ray ray = new Ray(Camera.main.transform.position, direction);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f))
            {
                Debug.DrawLine(ray.origin, hit.point, Color.yellow, 1f);

                EnemyDetector enemy = hit.collider.GetComponent<EnemyDetector>();
                if (enemy != null)
                {
                    enemy.OnHit(shotgunPelletDamage);
                }
                else
                {
                    Debug.Log($"Shotgun pellet hit {hit.collider.name}, no EnemyDetector.");
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
            Debug.Log($"Reloaded. New ammo: {currentAmmo}/{reserveAmmo}");
            UpdateAmmoUI();
        }
    }

    void UpdateAmmoUI()
    {
        if (currentAmmoText != null)
            currentAmmoText.text = currentAmmo.ToString();
        else
            Debug.LogWarning("currentAmmoText is not assigned.");

        if (reserveAmmoText != null)
            reserveAmmoText.text = reserveAmmo.ToString();
        else
            Debug.LogWarning("reserveAmmoText is not assigned.");
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

        Debug.Log($"Picked up {amount} {type} ammo.");
    }
}
