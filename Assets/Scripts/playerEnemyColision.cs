using UnityEngine;

public class PlayerTeleportOnCollision : MonoBehaviour
{
    public Vector3 defaultPosition = new Vector3(0, 0, 0);  // Domy�lna pozycja gracza

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Sprawdzenie, czy gracz zderzy� si� z przeciwnikiem
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TeleportToDefaultPosition();
        }
    }

    void TeleportToDefaultPosition()
    {
        // Teleportacja gracza na domy�ln� pozycj�
        transform.position = defaultPosition;
    }
}
