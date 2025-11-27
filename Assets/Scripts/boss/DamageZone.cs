using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DamageZone2D : MonoBehaviour
{
    [Header("Damage")]
    public int damagePerTick = 5;
    public float tickInterval = 0.30f;     // co ile sekund kolejne obrażenia podczas przebywania w strefie
    public bool damageOnEnter = true;      // czy zadać od razu przy wejściu

    [Header("Filter")]
    public string requiredTag = "Player";  // zostaw puste, jeśli nie chcesz filtrować po tagu

    [Header("Optional knockback")]
    public float knockbackForce = 0f;      // >0 aby włączyć
    public Vector2 fixedDirection = Vector2.zero; // (0,0) = od strefy do gracza; inaczej stały kierunek

    private readonly Dictionary<PlayerHealth, float> _nextTickTime = new();

    void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true; // to ma być TRIGGER
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!Accept(other)) return;

        var ph = other.GetComponentInParent<PlayerHealth>();
        if (ph == null) return;

        if (damageOnEnter)
            DoDamage(ph, other);

        // ustaw pierwszy termin na przyszły tick
        _nextTickTime[ph] = Time.time + tickInterval;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!Accept(other)) return;

        var ph = other.GetComponentInParent<PlayerHealth>();
        if (ph == null) return;

        float tNext;
        if (!_nextTickTime.TryGetValue(ph, out tNext)) tNext = 0f;

        if (Time.time >= tNext)
        {
            DoDamage(ph, other);
            _nextTickTime[ph] = Time.time + tickInterval;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        var ph = other.GetComponentInParent<PlayerHealth>();
        if (ph != null) _nextTickTime.Remove(ph);
    }

    bool Accept(Collider2D other)
    {
        if (!string.IsNullOrEmpty(requiredTag) && !other.CompareTag(requiredTag))
            return false;
        return true;
    }

    void DoDamage(PlayerHealth ph, Collider2D hitCol)
    {
        ph.TakeDamage(damagePerTick);

        if (knockbackForce > 0f)
        {
            var rb = hitCol.attachedRigidbody ?? ph.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 dir = fixedDirection;
                if (dir == Vector2.zero)
                {
                    // od centrum strefy do gracza
                    dir = ((Vector2)rb.worldCenterOfMass - (Vector2)transform.position).normalized;
                }
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y); // nie zerujemy
                rb.AddForce(dir.normalized * knockbackForce, ForceMode2D.Impulse);
            }
        }
    }
}
