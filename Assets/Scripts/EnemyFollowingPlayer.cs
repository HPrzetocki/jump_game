using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    public Transform player;         // Przeciwnik �ledzi ten obiekt (np. gracza)
    public float speed = 2f;         // Pr�dko�� poruszania
    private Rigidbody2D rb;
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (player == null) return;

        // Oblicz kierunek do gracza
        Vector2 direction = (player.position - transform.position).normalized;

        // Zmie� kierunek tylko w poziomie (�eby nie podskakiwa�)
        movement = new Vector2(direction.x, 0);

        // Porusz przeciwnika
        rb.linearVelocity = new Vector2(movement.x * speed, rb.linearVelocity.y);

        // Obracaj przeciwnika w stron� gracza
        if (movement.x != 0)
        {
            transform.localScale = new Vector3(movement.x > 0 ? 1 : -1, 1, 1);
        }
    }
}
