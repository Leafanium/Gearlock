using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTarget : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // Death Animation Coroutine
    public IEnumerator Die()
    {
        // Rotate the game object as if it fell over
        this.transform.Rotate(-75, 0, 0);

        // Wait for a few seconds
        yield return new WaitForSeconds(1.5f);

        //
        Destroy(gameObject);

    }

    public void ReactToHit()
    {
        // Get reference to wandering AI scripy
        // Pass in False if such a scripy is attached
        WanderingAI behavior = GetComponent<WanderingAI>();
        if (behavior != null)
        {
            behavior.SetAlive(false);

            StartCoroutine(Die());
        }


    }
}
