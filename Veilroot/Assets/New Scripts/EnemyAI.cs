using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
    public enum State { Climbing, Shooting, Falling, Stunned }

    [Header("References")]
    public Transform player;
    public Rigidbody rb;
    public Transform firePoint;

    [Header("Climb Targets")]
    public Transform[] climbTargets;              // Assigned at spawn — can be 1 or more
    public Transform[] allClimbTargets;           // Optional full set for re-randomizing

    [Header("Health")]
    public float maxHealth = 100f;
    public float downThreshold = 30f;
    private float currentHealth;

    [Header("Movement")]
    public float climbSpeed = 2.5f;
    public float perchDistance = 2f;
    public float driftIntensity = 0.3f;            // For side wobble

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

    private Vector3 driftOffset;
    private float driftTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentHealth = maxHealth;

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        GenerateNewDrift();
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
                // Await shotgun finish
                break;
        }
    }

    void ClimbTowardTarget()
    {
        if (climbTargets == null || climbTargets.Length == 0) return;

        Transform target = climbTargets[0];
        Vector3 targetDir = (target.position - transform.position).normalized;

        // Raycast ahead to find the surface we're supposed to climb
        RaycastHit hit;
        if (Physics.Raycast(transform.position, targetDir, out hit, 5f))
        {
            Vector3 wallNormal = hit.normal;

            // Calculate a climb direction along the wall surface
            Vector3 climbDir = Vector3.Cross(wallNormal, transform.right).normalized;

            // Offset from wall by 2.5 units
            Vector3 offset = wallNormal * 2.5f;
            Vector3 desiredPos = hit.point + offset;

            // Move toward the surface-aligned offset
            transform.position = Vector3.MoveTowards(transform.position, desiredPos, climbSpeed * Time.deltaTime);

            // Rotate to face the wall properly
            transform.rotation = Quaternion.LookRotation(-wallNormal, Vector3.up);
        }
        else
        {
            // No wall detected ahead — fallback to move toward target raw
            transform.position += targetDir * (climbSpeed * 0.5f) * Time.deltaTime;
        }

        // Stop if close enough
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

        // Optional: Re-randomize climb target from full list
        if (allClimbTargets != null && allClimbTargets.Length > 0)
        {
            climbTargets = new Transform[] { allClimbTargets[Random.Range(0, allClimbTargets.Length)] };
        }

        state = State.Climbing;
        fallTimer = 0f;
        isRecovering = false;
    }

    public void OnHit(float damage)
    {
        currentHealth -= damage;

        if (state == State.Falling)
        {
            fallTimer = 0f; // reset timer if re-hit while down
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

    void GenerateNewDrift()
    {
        driftOffset = new Vector3(
            Random.Range(-driftIntensity, driftIntensity),
            Random.Range(-driftIntensity * 0.25f, driftIntensity * 0.25f), // Slight vertical drift
            0f
        );
    }
}
