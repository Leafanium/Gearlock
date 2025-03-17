using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : MonoBehaviour
{
    [SerializeField] private int healAmount = 20;
    [SerializeField] private AudioClip pickupSound;  // Sound when health pack is collected
    [SerializeField] private AudioClip healthFullSound; // Sound when health is already full
    [SerializeField] private GameObject pickupEffect;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerCharacter playerCharacter = other.GetComponent<PlayerCharacter>();

            if (playerCharacter != null)
            {
                if (playerCharacter.CanHeal()) // Player needs healing
                {
                    playerCharacter.Heal(healAmount);

                    // Play the pickup sound
                    if (pickupSound != null)
                    {
                        AudioSource.PlayClipAtPoint(pickupSound, transform.position);
                    }

                    // Spawn the pickup effect
                    if (pickupEffect != null)
                    {
                        Instantiate(pickupEffect, transform.position, Quaternion.identity);
                    }

                    Destroy(gameObject); // Remove the health pack
                }
                else // Player is already at full health
                {
                    Debug.Log("Player health is already full!");

                    // Play the "health full" sound
                    if (healthFullSound != null)
                    {
                        AudioSource.PlayClipAtPoint(healthFullSound, transform.position);
                    }
                }
            }
        }
    }
}
