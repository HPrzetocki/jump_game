using UnityEngine;

public class Bullet2 : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Debugowanie kolizji
        Debug.Log("Kolizja z: " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Zniszczenie przeciwnika
            Destroy(collision.gameObject);

            // Zniszczenie pocisku
            Destroy(gameObject);
        }
    }
}
