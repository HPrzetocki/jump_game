using UnityEngine;

[DefaultExecutionOrder(1000)] // uruchom PO większości skryptów (po Twoim ruchu)
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class AddPlatformMotion2D : MonoBehaviour
{
    [Header("Wykrywanie gruntu/platformy")]
    public LayerMask platformMask;      // ustaw warstwę platform (np. "Platforms")
    public float checkDepth = 0.06f;    // jak nisko pod stopami sprawdzamy

    Rigidbody2D rb;
    Collider2D col;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate; // płynniej
    }

    void FixedUpdate()
    {
        // 1) Szukamy platformy tuż POD stopami (nie kolizji, tylko overlap boksa)
        Bounds b = col.bounds;
        Vector2 size   = new Vector2(b.size.x * 0.95f, checkDepth);
        Vector2 center = new Vector2(b.center.x, b.min.y - checkDepth * 0.5f);

        var hits = Physics2D.OverlapBoxAll(center, size, 0f, platformMask);
        PlatformaTasmowa platforma = null;
        for (int i = 0; i < hits.Length; i++)
        {
            // bierzemy komponent z roota platformy
            var p = hits[i].GetComponentInParent<PlatformaTasmowa>();
            if (p != null) { platforma = p; break; }
        }

        // 2) Jeśli stoimy na platformie – dołóż jej deltę do pozycji gracza
        if (platforma != null && platforma.CurrentDelta != Vector2.zero)
        {
            rb.MovePosition(rb.position + platforma.CurrentDelta);
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!GetComponent<Collider2D>()) return;
        var b = GetComponent<Collider2D>().bounds;
        Vector2 size   = new Vector2(b.size.x * 0.95f, checkDepth);
        Vector2 center = new Vector2(b.center.x, b.min.y - checkDepth * 0.5f);
        Gizmos.color = new Color(0,1,1,0.25f);
        Gizmos.DrawCube(center, size);
        Gizmos.color = new Color(0,1,1,1);
        Gizmos.DrawWireCube(center, size);
    }
#endif
}
