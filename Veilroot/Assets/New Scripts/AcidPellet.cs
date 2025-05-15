using UnityEngine;
using CodeMonkey.HealthSystemCM;

public class AcidPellet : MonoBehaviour
{
    public float speed = 30f;
    public float lifetime = 5f;
    public float damage = 10f;

    private Rigidbody rb;
    private bool launched = false;
    private Vector3 launchDir;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, lifetime); // auto-destroy to avoid clutter
    }

    public void Launch(Vector3 direction)
    {
        launchDir = direction.normalized;
        launched = true;
    }

    void FixedUpdate()
    {
        if (launched && rb != null)
        {
            rb.linearVelocity = launchDir * speed;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            if (HealthSystem.TryGetHealthSystem(collision.collider.gameObject, out HealthSystem playerHealth))
            {
                playerHealth.Damage(damage);
            }
        }

        Destroy(gameObject);
    }
}
