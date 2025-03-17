using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour {

    private int maxHealth = 100;
    private int currentHealth;

    void Start() {

        currentHealth = maxHealth;
    }


    public bool CanHeal() {

        return currentHealth < maxHealth;

    }

    public void Heal(int amount) {
        
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log("Healed! Health is:" + currentHealth);
    }
}

