using System.Collections;
using UnityEngine;

public class BossWeapon : MonoBehaviour
{
    public int attackDamage = 20;
    public int enragedAttackDamage = 40;

    public Vector3 attackOffset;
    public float attackRange = 1f;
    public LayerMask attackMask;

    public GameObject projectilePrefab;
    public float projectileSpeed = 25f;

    public int burstCount = 5;              // liczba pocisków w serii
    public float burstInterval = 0.1f;      // odstęp między pociskami

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

            Vector3 direction = (player.position - transform.position).normalized;

            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Rigidbody2D projRb = projectile.GetComponent<Rigidbody2D>();
            if (projRb != null)
            {
                projRb.velocity = direction * projectileSpeed;
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
}
