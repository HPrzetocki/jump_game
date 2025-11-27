using UnityEngine;

public class CarryOnTopSimple : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.collider.CompareTag("Player")) return;
        Vector2 n = Vector2.zero; foreach (var c in col.contacts) n += c.normal; n /= Mathf.Max(1, col.contactCount);
        if (n.y > 0.5f) col.transform.SetParent(transform, true);
    }
    void OnCollisionExit2D(Collision2D col)
    {
        if (col.collider.CompareTag("Player")) col.transform.SetParent(null, true);
    }
}
