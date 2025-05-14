using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    public enum WeaponType { SMG, Shotgun }
    public WeaponType type;
    public int amount = 10;

    private void OnTriggerEnter(Collider other)
    {
        WeaponSwitcher weapon = other.GetComponent<WeaponSwitcher>();
        if (weapon != null)
        {
            weapon.PickupAmmo(type, amount);
            Destroy(gameObject); // disappear pickup
        }
    }
}
