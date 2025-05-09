using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab;  // Prefab pocisku
    public Transform firePoint;      // Punkt, z którego ma być wystrzelony pocisk
    public float bulletSpeed = 10f;  // Prędkość pocisku
    public float bulletOffset = 0.5f; // Przesunięcie pocisku w zależności od kierunku (lewo/prawo)

    public float cooldownTime = 1f; // Czas cooldownu (w sekundach)
    private float lastShootTime = 0f; // Czas, kiedy ostatni raz strzelano

    private int direction = 1; // 1 oznacza w prawo, -1 w lewo

    void Update()
    {
        // Sprawdzamy, czy minął czas cooldownu i czy naciśnięto przycisk strzału (np. Left Shift)
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= lastShootTime + cooldownTime)
        {
            Shoot();
        }

        // Ustawiamy kierunek strzału na podstawie ruchu gracza
        if (Input.GetKey(KeyCode.A)) // Przycisk w lewo
        {
            direction = -1;
        }
        else if (Input.GetKey(KeyCode.D)) // Przycisk w prawo
        {
            direction = 1;
        }
    }

    void Shoot()
    {
        // Ustalamy pozycję pocisku w zależności od kierunku
        Vector3 bulletPosition = firePoint.position + new Vector3(bulletOffset * direction, 0, 0);

        // Tworzymy pocisk
        GameObject bullet = Instantiate(bulletPrefab, bulletPosition, firePoint.rotation);

        // Uzyskujemy komponent Rigidbody2D pocisku, aby nadać mu prędkość
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

        // Ustalamy prędkość pocisku w zależności od kierunku
        bulletRb.linearVelocity = new Vector2(bulletSpeed * direction, 0);

        // Aktualizujemy czas ostatniego strzału
        lastShootTime = Time.time;
    }
}
