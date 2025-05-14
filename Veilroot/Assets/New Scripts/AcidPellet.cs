using UnityEngine;

public class AcidPellet : MonoBehaviour
{
    public float speed = 8f;
    public float damage = 10f;
    public float lifetime = 5f;
    public GameObject impactEffect; // Optional VFX prefab

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, lifetime);
    }

    public void Launch(Vector3 direction)
    {
        rb.linearVelocity = direction * speed;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var healthComp = collision.gameObject.GetComponent<CodeMonkey.HealthSystemCM.HealthSystemComponent>();
            if (healthComp != null)
            {
                healthComp.GetHealthSystem().Damage(damage);
            }
        }

        if (impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
