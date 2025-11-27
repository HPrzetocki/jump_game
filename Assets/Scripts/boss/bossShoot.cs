using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    public Transform player;           // Referencja do gracza
    public GameObject bulletPrefab;    // Prefab pocisku
    public Transform firePoint;        // Miejsce, z którego wystrzeliwany jest pocisk
    public float shootRange = 10f;     // Zasięg wykrywania gracza
    public float fireRate = 1f;        // Częstotliwość strzałów (strzały na sekundę)
    public float bulletSpeed = 10f;    // Prędkość pocisków

    private float fireCooldown = 0f;

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Sprawdzenie odległości gracza
        if (distanceToPlayer <= shootRange)
        {
            // Obracamy tylko w osi Y (żeby boss nie przewracał się)
            Vector3 direction = (player.position - transform.position).normalized;
            Vector3 flatDirection = new Vector3(direction.x, 0f, direction.z); // ignorujemy Y
            if (flatDirection != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(flatDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            }

            // Strzelanie z cooldownem
            if (fireCooldown <= 0f)
            {
                Shoot(flatDirection);
                fireCooldown = 1f / fireRate;
            }
        }

        // Odliczanie cooldownu
        if (fireCooldown > 0f)
        {
            fireCooldown -= Time.deltaTime;
        }
    }

    void Shoot(Vector3 direction)
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = direction.normalized * bulletSpeed;
        }
        else
        {
            Debug.LogWarning("Brak Rigidbody na pocisku!");
        }
    }
}
