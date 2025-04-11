using UnityEngine;

public class PlayerTeleportOnCollision : MonoBehaviour
{
    public Vector3 defaultPosition = new Vector3(0, 0, 0);  // Domyœlna pozycja gracza

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Sprawdzenie, czy gracz zderzy³ siê z przeciwnikiem
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TeleportToDefaultPosition();
        }
    }

    void TeleportToDefaultPosition()
    {
        // Teleportacja gracza na domyœln¹ pozycjê
        transform.position = defaultPosition;
    }
}
