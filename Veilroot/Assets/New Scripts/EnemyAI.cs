using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    public enum State { Climbing, Shooting, Falling, Stunned }

    [Header("References")]
    public Transform player;
    public Rigidbody rb;
    public Transform firePoint;
    public Transform[] climbTargets;

    [Header("Health")]
    public float maxHealth = 100f;
    public float downThreshold = 30f;
    private float currentHealth;

    [Header("Movement")]
    public float climbSpeed = 2f;
    public float perchDistance = 2f;

    [Header("Shooting")]
    public GameObject acidPrefab;
    public float projectileSpeed = 8f;
    public float shootCooldown = 2f;

    [Header("Recovery")]
    public float recoveryTime = 5f;

    private State state = State.Climbing;
    private float shootTimer;
    private float fallTimer;
    private bool isRecovering = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentHealth = maxHealth;

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        switch (state)
        {
            case State.Climbing:
                ClimbTowardTarget();
                break;
            case State.Shooting:
                HandleShooting();
                break;
            case State.Falling:
                fallTimer += Time.deltaTime;
                if (fallTimer >= recoveryTime && !isRecovering)
                {
                    StartCoroutine(RecoverToClimb());
                }
                break;
            case State.Stunned:
                // Waiting for close shotgun hit
                break;
        }
    }

    void ClimbTowardTarget()
    {
        if (climbTargets.Length == 0) return;

        Transform target = climbTargets[Random.Range(0, climbTargets.Length)];
        Vector3 dir = (target.position - transform.position).normalized;

        transform.position += dir * climbSpeed * Time.deltaTime;

        // Wall alignment
        if (Physics.Raycast(transform.position, -transform.forward, out RaycastHit hit, 2f))
        {
            transform.rotation = Quaternion.LookRotation(-hit.normal, Vector3.up);
        }

        if (Vector3.Distance(transform.position, target.position) <= perchDistance)
        {
            state = State.Shooting;
            shootTimer = shootCooldown;
        }
    }

    void HandleShooting()
    {
        if (player == null) return;

        shootTimer -= Time.deltaTime;

        if (shootTimer <= 0f)
        {
            Vector3 toPlayer = (player.position - firePoint.position).normalized;
            GameObject acid = Instantiate(acidPrefab, firePoint.position, Quaternion.LookRotation(toPlayer));
            AcidPellet pellet = acid.GetComponent<AcidPellet>();
            if (pellet != null)
                pellet.Launch(toPlayer);

            shootTimer = shootCooldown;
        }
    }

    IEnumerator RecoverToClimb()
    {
        isRecovering = true;
        rb.isKinematic = true;
        rb.useGravity = false;

        yield return new WaitForSeconds(0.1f);

        state = State.Climbing;
        fallTimer = 0f;
        isRecovering = false;
    }

    public void OnHit(float damage)
    {
        currentHealth -= damage;

        if (state == State.Falling)
        {
            fallTimer = 0f; // reset fall timer if hit again
        }

        if (currentHealth <= 0f && state == State.Stunned)
        {
            Die();
        }
        else if (currentHealth <= downThreshold && state != State.Falling && state != State.Stunned)
        {
            EnterFall();
        }
    }

    void EnterFall()
    {
        state = State.Falling;
        rb.useGravity = true;
        rb.isKinematic = false;
        fallTimer = 0f;
    }

    public void EnterStunnedState()
    {
        state = State.Stunned;
        rb.useGravity = false;
        rb.isKinematic = true;
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
