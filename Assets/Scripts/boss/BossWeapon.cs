using System.Collections;
using UnityEngine;

public class BossWeapon : MonoBehaviour
{
    public int attackDamage = 20;
    public int enragedAttackDamage = 20;

    public Vector3 attackOffset;
    public float attackRange = 1f;
    public LayerMask attackMask;

    public GameObject projectilePrefab;
    public float projectileSpeed = 25f;

    public int burstCount = 5;              // liczba pocisków w serii
    public float burstInterval = 0.1f;      // odstęp między pociskamipublic float rangedAttackCooldown = 3f;  // czas pomiędzy seriami
    public float rangedAttackCooldown = 3f;  // czas pomiędzy seriami
    public float rangedAttackRange = 15f;    // zasięg strzału

    private float nextAttackTime = 0f;

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void RangedAttack()
    {
        if (player == null) return;
        StartCoroutine(RangedAttackBurst());
    }

private IEnumerator RangedAttackBurst()
{
    for (int i = 0; i < burstCount; i++)
    {
        if (player == null) yield break;

        // Rzutowanie pozycji na Vector2 (ignoruje Z)
        Vector2 enemyPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 playerPos = new Vector2(player.position.x, player.position.y);
        Vector2 direction = (playerPos - enemyPos).normalized;

        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Rigidbody2D projRb = projectile.GetComponent<Rigidbody2D>();

        if (projRb != null)
        {
            projRb.linearVelocity = direction * projectileSpeed;
        }
        else
        {
            Debug.LogWarning("Brak Rigidbody2D na pocisku!");
        }

        yield return new WaitForSeconds(burstInterval);
    }
}



    public void Attack()
    {
        Vector3 pos = transform.position + transform.right * attackOffset.x + transform.up * attackOffset.y;
        Collider2D colInfo = Physics2D.OverlapCircle(pos, attackRange, attackMask);
        if (colInfo != null)
        {
            colInfo.GetComponent<PlayerHealth>()?.TakeDamage(attackDamage);
        }
    }

    public void EnragedAttack()
    {
        Vector3 pos = transform.position + transform.right * attackOffset.x + transform.up * attackOffset.y;
        Collider2D colInfo = Physics2D.OverlapCircle(pos, attackRange, attackMask);
        if (colInfo != null)
        {
            colInfo.GetComponent<PlayerHealth>()?.TakeDamage(enragedAttackDamage);
        }
    }

    void OnDrawGizmosSelected()
    {
        Vector3 pos = transform.position + transform.right * attackOffset.x + transform.up * attackOffset.y;
        Gizmos.DrawWireSphere(pos, attackRange);
    }

    void Update()
{
    if (player == null) return;

    float distanceToPlayer = Vector2.Distance(transform.position, player.position);

    // Jeśli gracz jest w zasięgu i czas pozwala
    if (distanceToPlayer <= rangedAttackRange && Time.time >= nextAttackTime)
    {
        RangedAttack();
        nextAttackTime = Time.time + rangedAttackCooldown;
    }
}
    
}
