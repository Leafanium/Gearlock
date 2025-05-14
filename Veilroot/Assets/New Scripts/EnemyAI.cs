using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    public enum State { Crawling, Shooting, Falling, Stunned }

    [Header("Core References")]
    public Transform player;
    public Transform firePoint;
    public Transform[] climbTargets;          // Assigned by spawner
    public Transform[] allClimbTargets;       // For re-randomizing on recovery
    private Transform currentTarget;

    private Rigidbody rb;
    private State state = State.Crawling;

    [Header("Surface Detection")]
    public LayerMask surfaceMask;
    public float surfaceCheckRadius = 1.5f;
    public float minSurfaceDistance = 1.8f;
    public float maxSurfaceDistance = 2.2f;

    [Header("Movement")]
    public float crawlSpeed = 3f;
    public float climbRotateSpeed = 10f;
    public float targetArriveThreshold = 2f;

    [Header("Shooting")]
    public GameObject acidPrefab;
    public float shootCooldown = 2f;
    private float shootTimer;

    [Header("Health")]
    public float maxHealth = 100f;
    public float downThreshold = 30f;
    private float currentHealth;

    [Header("Recovery")]
    public float recoveryTime = 5f;
    private float fallTimer;
    private bool isRecovering = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
        currentHealth = maxHealth;

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        // Pick a climb target
        if (climbTargets != null && climbTargets.Length > 0)
        {
            currentTarget = climbTargets[Random.Range(0, climbTargets.Length)];
        }
    }

    void Update()
    {
        switch (state)
        {
            case State.Crawling:
                AdaptiveCrawl();
                break;
            case State.Shooting:
                HandleShooting();
                break;
            case State.Falling:
                fallTimer += Time.deltaTime;
                if (fallTimer >= recoveryTime && !isRecovering)
                    StartCoroutine(RecoverToCrawl());
                break;
        }
    }

    void AdaptiveCrawl()
    {
        if (!currentTarget) return;

        if (Physics.SphereCast(transform.position, surfaceCheckRadius, -transform.up, out RaycastHit hit, maxSurfaceDistance * 2f, surfaceMask))
        {
            Vector3 surfaceNormal = hit.normal;
            float distFromSurface = hit.distance;

            // Adjust hover offset
            if (distFromSurface < minSurfaceDistance)
                transform.position += surfaceNormal * (minSurfaceDistance - distFromSurface);
            else if (distFromSurface > maxSurfaceDistance)
                transform.position -= surfaceNormal * (distFromSurface - maxSurfaceDistance);

            // Move along surface toward target
            Vector3 toTarget = (currentTarget.position - transform.position).normalized;
            Vector3 moveDir = Vector3.ProjectOnPlane(toTarget, surfaceNormal).normalized;
            transform.position += moveDir * crawlSpeed * Time.deltaTime;

            // Align rotation
            Quaternion targetRot = Quaternion.LookRotation(moveDir, surfaceNormal);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * climbRotateSpeed);

            // Reached target
            if (Vector3.Distance(transform.position, currentTarget.position) <= targetArriveThreshold)
            {
                state = State.Shooting;
                shootTimer = shootCooldown;
            }
        }
        else
        {
            // Drop slowly if no surface detected
            transform.position += Vector3.down * crawlSpeed * Time.deltaTime;
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

    public void OnHit(float damage)
    {
        currentHealth -= damage;

        if (state == State.Falling)
            fallTimer = 0f;

        if (currentHealth <= 0f && state == State.Stunned)
        {
            Destroy(gameObject);
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

    IEnumerator RecoverToCrawl()
    {
        isRecovering = true;
        rb.useGravity = false;
        rb.isKinematic = true;

        yield return new WaitForSeconds(0.1f);

        if (allClimbTargets != null && allClimbTargets.Length > 0)
            currentTarget = allClimbTargets[Random.Range(0, allClimbTargets.Length)];

        state = State.Crawling;
        isRecovering = false;
    }

    public void EnterStunnedState()
    {
        state = State.Stunned;
        rb.useGravity = false;
        rb.isKinematic = true;
    }
}
