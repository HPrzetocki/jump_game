using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float speed = 2f;
    public float moveTime = 2f;

    private bool movingRight = true;
    private float moveTimer;

    void Start()
    {
        moveTimer = moveTime;
    }

    void Update()
    {
        // Poruszanie przeciwnika w odpowiednim kierunku
        Vector2 direction = movingRight ? Vector2.right : Vector2.left;
        transform.Translate(direction * speed * Time.deltaTime);

        // Odliczanie czasu do zmiany kierunku
        moveTimer -= Time.deltaTime;
        if (moveTimer <= 0f)
        {
            Flip();
            moveTimer = moveTime;
        }
    }

    void Flip()
    {
        movingRight = !movingRight;

        // Obrót sprite'a, jeœli chcesz ¿eby siê "obraca³"
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
