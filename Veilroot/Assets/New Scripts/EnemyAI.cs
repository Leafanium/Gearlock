using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    public enum State { Crawling, Shooting, Falling, Stunned }

    [Header("References")]
    public Transform player;
    public Transform firePoint;
    public Transform[] climbTargets;
    public Transform[] allClimbTargets;
    private Transform currentTarget;

    private Rigidbody rb;
    private State state = State.Crawling;

    [Header("Surface Detection")]
    public LayerMask surfaceMask;
    public float surfaceRayDistance = 5f;
    public float surfaceOffset = 4f;

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
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (climbTargets != null && climbTargets.Length > 0)
            currentTarget = climbTargets[Random.Range(0, climbTargets.Length)];

        EnemyTracker.activeEnemyCount++;
    }

    void Update()
    {
        switch (state)
        {
            case State.Crawling:
                CrawlToTarget();
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

    void CrawlToTarget()
    {
        if (!currentTarget) return;

        Vector3 toTarget = (currentTarget.position - transform.position).normalized;

        if (Physics.Raycast(transform.position, toTarget, out RaycastHit hit, surfaceRayDistance, surfaceMask))
        {
            Vector3 wallNormal = hit.normal;
            Vector3 surfaceAlignedDir = Vector3.Cross(wallNormal, transform.right).normalized;
            Vector3 targetPos = hit.point + wallNormal * surfaceOffset;

            transform.position = Vector3.MoveTowards(transform.position, targetPos, crawlSpeed * Time.deltaTime);
            Quaternion targetRot = Quaternion.LookRotation(-wallNormal, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * climbRotateSpeed);
        }
        else
        {
            transform.position += toTarget * crawlSpeed * Time.deltaTime;
        }

        if (Vector3.Distance(transform.position, currentTarget.position) <= targetArriveThreshold)
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
            if (pellet != null) pellet.Launch(toPlayer);
            shootTimer = shootCooldown;
        }
    }

    public void OnHit(float damage, string weaponType)
    {
        if (state == State.Stunned) return;

        if (state == State.Crawling || state == State.Shooting)
        {
            currentHealth -= damage;

            if (currentHealth <= downThreshold && weaponType == "SMG")
            {
                EnterFall();
            }
        }
        else if (state == State.Falling && weaponType == "Shotgun")
        {
            EnterStunnedState();
            ActivateRagdoll(); // Optional
            Die();
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
        yield return new WaitForSeconds(recoveryTime);

        if (state == State.Falling)
        {
            rb.useGravity = false;
            rb.isKinematic = true;

            if (allClimbTargets != null && allClimbTargets.Length > 0)
                currentTarget = allClimbTargets[Random.Range(0, allClimbTargets.Length)];

            state = State.Crawling;
        }

        isRecovering = false;
    }

    void EnterStunnedState()
    {
        state = State.Stunned;
        rb.useGravity = false;
        rb.isKinematic = true;
    }

    void Die()
    {
        EnemyTracker.activeEnemyCount--;
        VictoryManager.CheckForWin();
        Destroy(gameObject, 1.5f);
    }

    void OnDestroy()
    {
        if (state != State.Stunned) // Clean up count if killed another way
            EnemyTracker.activeEnemyCount--;
    }

    void ActivateRagdoll()
    {
        Animator animator = GetComponent<Animator>();
        if (animator) animator.enabled = false;

        Rigidbody[] limbs = GetComponentsInChildren<Rigidbody>();
        foreach (var limb in limbs)
        {
            limb.isKinematic = false;
            limb.useGravity = true;
        }

        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (var col in colliders)
            col.enabled = true;

        rb.isKinematic = true;
        if (GetComponent<Collider>()) GetComponent<Collider>().enabled = false;
    }
}
