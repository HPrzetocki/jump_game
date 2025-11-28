using System.Collections;
using UnityEngine;

public class BossGun : MonoBehaviour
{
    [Header("Obrażenia wręcz")]
    public int baseMeleeDamage = 20;
    public int enragedMeleeDamage = 20;

    public Vector3 meleeAttackOffset;
    public float meleeAttackRange = 1f;
    public LayerMask meleeAttackMask;

    [Header("Strzały dystansowe")]
    public GameObject projectilePrefab;
    public float shotSpeed = 25f;

    public int shotsInBurst = 5;          // liczba pocisków w serii
    public float timeBetweenShots = 0.1f; // odstęp między pociskami

    public float rangedAttackCooldown = 3f;  // czas pomiędzy seriami
    public float rangedAttackRange = 15f;    // zasięg strzału

    [Header("Lufa / punkt strzału")]
    public Transform barrelPivot;           // obiekt lufy, który będzie się obracał
    public Transform muzzlePoint;           // miejsce, z którego wylatuje pocisk (może być dzieckiem lufy)

    private float nextRangedAttackTime = 0f;
    private Transform playerTarget;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTarget = playerObj.transform;
        }
    }

    void Update()
    {
        if (playerTarget == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);

        // Jeśli gracz jest w zasięgu i cooldown minął
        if (distanceToPlayer <= rangedAttackRange && Time.time >= nextRangedAttackTime)
        {
            StartRangedBurst();
            nextRangedAttackTime = Time.time + rangedAttackCooldown;
        }

        // Obracanie samej lufy w stronę gracza (ciągle)
        RotateBarrelTowardsPlayer();
    }

    // ===== STRZAŁY DISTANSOWE =====

    public void StartRangedBurst()
    {
        if (playerTarget == null) return;
        StartCoroutine(RangedBurstCoroutine());
    }

    private IEnumerator RangedBurstCoroutine()
    {
        for (int i = 0; i < shotsInBurst; i++)
        {
            if (playerTarget == null) yield break;

            // Pozycje 2D (ignorujemy Z)
            Vector2 gunPos = (barrelPivot != null ? (Vector2)barrelPivot.position : (Vector2)transform.position);
            Vector2 playerPos = new Vector2(playerTarget.position.x, playerTarget.position.y);
            Vector2 direction = (playerPos - gunPos).normalized;

            // Obróć lufę pod aktualny strzał
            RotateBarrelTowardsDirection(direction);

            // Punkt startu pocisku: jeśli jest muzzlePoint, używamy jego, inaczej pivot
            Vector3 spawnPos = muzzlePoint != null ? muzzlePoint.position : gunPos;

            GameObject projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
            Rigidbody2D projRb = projectile.GetComponent<Rigidbody2D>();

            if (projRb != null)
            {
                projRb.linearVelocity = direction * shotSpeed;
            }
            else
            {
                Debug.LogWarning("Brak Rigidbody2D na pocisku!");
            }

            yield return new WaitForSeconds(timeBetweenShots);
        }
    }

    private void RotateBarrelTowardsPlayer()
    {
        if (barrelPivot == null || playerTarget == null) return;

        Vector2 gunPos = barrelPivot.position;
        Vector2 playerPos = playerTarget.position;
        Vector2 dir = (playerPos - gunPos).normalized;

        RotateBarrelTowardsDirection(dir);
    }

    private void RotateBarrelTowardsDirection(Vector2 dir)
    {
        if (barrelPivot == null) return;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        barrelPivot.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    // ===== ATAKI WRĘCZ =====

    public void MeleeAttack()
    {
        Vector3 pos = transform.position +
                      transform.right * meleeAttackOffset.x +
                      transform.up * meleeAttackOffset.y;

        Collider2D colInfo = Physics2D.OverlapCircle(pos, meleeAttackRange, meleeAttackMask);
        if (colInfo != null)
        {
            colInfo.GetComponent<PlayerHealth>()?.TakeDamage(baseMeleeDamage);
        }
    }

    public void EnragedMeleeAttack()
    {
        Vector3 pos = transform.position +
                      transform.right * meleeAttackOffset.x +
                      transform.up * meleeAttackOffset.y;

        Collider2D colInfo = Physics2D.OverlapCircle(pos, meleeAttackRange, meleeAttackMask);
        if (colInfo != null)
        {
            colInfo.GetComponent<PlayerHealth>()?.TakeDamage(enragedMeleeDamage);
        }
    }

    void OnDrawGizmosSelected()
    {
        Vector3 pos = transform.position +
                      transform.right * meleeAttackOffset.x +
                      transform.up * meleeAttackOffset.y;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pos, meleeAttackRange);
    }
}
