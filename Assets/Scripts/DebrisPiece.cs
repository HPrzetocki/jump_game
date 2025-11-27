using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DebrisPiece : MonoBehaviour
{
    public float lifeTime = 5f;         // auto-despawn
    public bool addSplashImpulse = true;
    public float splashRadius = 0.7f;   // promień efektu przy uderzeniu w ziemię
    public float splashForce = 4f;
    public LayerMask playerMask;

    Rigidbody2D rb;
    float t0;
    bool spawned;

    void Awake(){ rb = GetComponent<Rigidbody2D>(); }

    public void Spawn(Vector2 startPos, float xDrift = 0f)
    {
        transform.position = startPos;
        gameObject.SetActive(true);
        t0 = Time.time;
        spawned = true;

        rb.velocity = new Vector2(xDrift, 0f);
        rb.angularVelocity = Random.Range(-180f, 180f);
    }

    void Update()
    {
        if (spawned && Time.time - t0 >= lifeTime)
            Despawn();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        // 1. Pierwsze uderzenie w ziemię -> splash (jednorazowo)
        if (addSplashImpulse)
        {
            addSplashImpulse = false;
            var hits = Physics2D.OverlapCircleAll(transform.position, splashRadius, playerMask);
            foreach (var h in hits)
            {
                var rbp = h.attachedRigidbody;
                if (rbp) rbp.AddForce(Vector2.up * splashForce, ForceMode2D.Impulse);
            }
        }
    }

    public void Despawn()
    {
        gameObject.SetActive(false);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (addSplashImpulse)
        {
            Gizmos.color = new Color(1,0.5f,0,0.25f);
            Gizmos.DrawSphere(transform.position, splashRadius);
        }
    }
#endif
}
