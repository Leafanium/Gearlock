using UnityEngine;
using CodeMonkey.HealthSystemCM;

[RequireComponent(typeof(Rigidbody))]
public class AcidPellet : MonoBehaviour
{
    public float speed = 100f;
    public float lifetime = 5f;
    public float damage = 10f;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.linearDamping = 0f;
        rb.mass = 0.1f;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void Launch(Vector3 direction)
    {
        if (rb == null)
        {
            Debug.LogError("Missing Rigidbody on AcidPellet.");
            return;
        }

        Vector3 force = direction.normalized * speed;
        rb.AddForce(force, ForceMode.VelocityChange);

        Debug.Log($"\uD83E\uDDEA Acid pellet force applied: {force}");
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
