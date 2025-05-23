using UnityEngine;
using System.Collections;

public class BossMovement : MonoBehaviour
{
    public float knockbackForce = 10f;
    public float jumpForce = 12f;
    public float freezeDuration = 0.5f;
    public float jumpDelay = 0.5f;
    public float playerDetectionRange = 3f;

    private Rigidbody2D rb;
    private Transform player;
    private BossWeapon bossWeapon;
    private bool isFrozen = false;
    private bool isPerformingSpecialAttack = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        bossWeapon = GetComponent<BossWeapon>();
    }

    void Update()
    {
        if (player != null && !isPerformingSpecialAttack)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer <= playerDetectionRange)
            {
                // losowa szansa na atak specjalny (np. 30%)
                if (Random.value < 0.6f)
                {
                    Vector2 awayDirection = (transform.position - player.position).normalized;
                    KnockbackInDirection(awayDirection);
                }
            }
        }
    }

    public void KnockbackInDirection(Vector2 directionAway)
    {
        if (rb == null || isFrozen) return;

        rb.velocity = Vector2.zero;
        rb.AddForce(directionAway * knockbackForce, ForceMode2D.Impulse);

        StartCoroutine(FreezeAndJump());
    }

    private IEnumerator FreezeAndJump()
    {
        isFrozen = true;
        isPerformingSpecialAttack = true;

        yield return new WaitForSeconds(freezeDuration);

        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;

        // 🚀 Strzał tylko w tym momencie
        bossWeapon?.RangedAttack();

        yield return new WaitForSeconds(jumpDelay);

        rb.bodyType = RigidbodyType2D.Dynamic;

        if (player != null)
        {
            Vector2 directionToPlayer = (player.position - transform.position).normalized;
            rb.AddForce(directionToPlayer * jumpForce, ForceMode2D.Impulse);
        }

        isFrozen = false;
        isPerformingSpecialAttack = false;
    }
}
