using UnityEngine;

public class GameController : MonoBehaviour
{
    Vector2 checkpointPos;
    Rigidbody2D playerRb;

    CameraController cameraController;

    private void Awake()
    {
        cameraController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();

        playerRb = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        checkpointPos = transform.position;
        playerRb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Die();
        }
    }
    public void UpadateCheckPoint(Vector2 pos)
    {
        checkpointPos = pos;
    }

    void Die()
    {
        Respawn();
    }

    void Respawn()
    {
        transform.position = checkpointPos;
    }
}

