using UnityEngine;

public class Elevator : MonoBehaviour
{
    public float moveDistance = 3f;     // Jak wysoko ma siê poruszaæ
    public float speed = 2f;            // Prêdkoœæ ruchu
    private Vector3 startPos;
    private Vector3 targetPos;
    private bool movingUp = true;

    void Start()
    {
        startPos = transform.position;
        targetPos = startPos + Vector3.up * moveDistance;
    }

    void Update()
    {
        Vector3 destination = movingUp ? targetPos : startPos;
        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);

        // Po dotarciu do celu — zmieñ kierunek
        if (Vector3.Distance(transform.position, destination) < 0.01f)
        {
            movingUp = !movingUp;
        }
    }
}