using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class MovingPlatformCarrier2D : MonoBehaviour
{
    [Header("Kogo wieziemy")]
    public LayerMask playerMask;      // ustaw warstwę gracza (np. Player)
    public string playerTag = "Player"; // dodatkowy filtr po tagu (opcjonalnie)

    [Header("Strefa wykrywania nad platformą")]
    public float detectHeight = 0.15f; // wysokość pudełka wykrywania nad górą platformy
    public float detectInset = 0.05f;  // zmniejszenie szerokości względem kolizji (żeby nie łapać obok)

    Rigidbody2D rb;
    Collider2D topCol;
    Vector2 lastPos;
    readonly HashSet<Rigidbody2D> passengers = new HashSet<Rigidbody2D>();

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        topCol = GetComponent<Collider2D>();
        lastPos = rb.position;
    }

    void FixedUpdate()
    {
        // delta ruchu platformy w tej klatce fizyki
        Vector2 current = rb.position;
        Vector2 delta = current - lastPos;

        // wykryj ciała stojące NA platformie (tuż nad jej górą)
        passengers.Clear();
        var b = topCol.bounds;

        // pudełko wykrywania lekko nad górą platformy
        Vector2 size = new Vector2(Mathf.Max(0.01f, b.size.x - detectInset * 2f), detectHeight);
        Vector2 center = new Vector2(b.center.x, b.max.y + detectHeight * 0.5f);

        var hits = Physics2D.OverlapBoxAll(center, size, 0f, playerMask);
        for (int i = 0; i < hits.Length; i++)
        {
            var h = hits[i];
            if (!string.IsNullOrEmpty(playerTag) && !h.CompareTag(playerTag)) continue;

            var prb = h.attachedRigidbody;
            if (prb != null) passengers.Add(prb);
        }

        // dołóż ten sam ruch platformy do każdego pasażera
        if (delta.sqrMagnitude > 0f && passengers.Count > 0)
        {
            foreach (var prb in passengers)
            {
                // MovePosition jest „fizyczne” i ładnie współpracuje z RB2D
                prb.MovePosition(prb.position + delta);
            }
        }

        lastPos = current;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!topCol) topCol = GetComponent<Collider2D>();
        var b = topCol.bounds;
        Vector2 size = new Vector2(Mathf.Max(0.01f, b.size.x - detectInset * 2f), detectHeight);
        Vector2 center = new Vector2(b.center.x, b.max.y + detectHeight * 0.5f);

        Gizmos.color = new Color(0f, 1f, 1f, 0.35f);
        Gizmos.DrawCube(center, size);
        Gizmos.color = new Color(0f, 1f, 1f, 1f);
        Gizmos.DrawWireCube(center, size);
    }
#endif
}
