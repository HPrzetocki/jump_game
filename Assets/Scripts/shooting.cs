using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab;  // Prefab pocisku
    public Transform firePoint;      // Punkt, z kt�rego ma by� wystrzelony pocisk
    public float bulletSpeed = 10f;  // Pr�dko�� pocisku
    public float bulletOffset = 0.5f; // Przesuni�cie pocisku w zale�no�ci od kierunku (lewo/prawo)

    void Update()
    {
        // Sprawdzamy, czy naci�ni�to przycisk do strza�u (np. Left Shift)
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // Ustalanie kierunku strza�u (w zale�no�ci od obrotu gracza)
        float direction = transform.eulerAngles.y == 0 ? 1 : -1;

        // Ustalamy pozycj� pocisku (w zale�no�ci od obrotu gracza)
        Vector3 bulletPosition = firePoint.position + new Vector3(bulletOffset * direction, 0, 0);

        // Tworzymy pocisk
        GameObject bullet = Instantiate(bulletPrefab, bulletPosition, firePoint.rotation);

        // Uzyskujemy komponent Rigidbody2D pocisku, aby nada� mu pr�dko��
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

        // Ustalamy pr�dko�� pocisku
        bulletRb.linearVelocity = new Vector2(bulletSpeed * direction, 0);
    }
}