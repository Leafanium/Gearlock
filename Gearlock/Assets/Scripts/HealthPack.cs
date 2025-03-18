using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : MonoBehaviour
{
    [SerializeField] private int healAmount = 20;
    [SerializeField] private AudioClip pickupSound;  // Pickup sound
    [SerializeField] private AudioClip healthFullSound; // Already Max Health sound
    [SerializeField] private GameObject pickupEffect;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerCharacter playerCharacter = other.GetComponent<PlayerCharacter>();

            if (playerCharacter != null)
            {
                if (playerCharacter.CanHeal()) // When you have to heal
                {
                    playerCharacter.Heal(healAmount);

                    
                    if (pickupSound != null)
                    {
                        AudioSource.PlayClipAtPoint(pickupSound, transform.position);
                    }

                    
                    if (pickupEffect != null)
                    {
                        Instantiate(pickupEffect, transform.position, Quaternion.identity);
                    }

                    Destroy(gameObject);
                }
                else
                {
                    Debug.Log("Player health is already full!");

                    
                    if (healthFullSound != null)
                    {
                        AudioSource.PlayClipAtPoint(healthFullSound, transform.position);
                    }
                }
            }
        }
    }
}
