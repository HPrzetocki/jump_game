using UnityEngine;

public class Checkpoint : MonoBehaviour
{
   GameController gameController;

    private void Awake()
    {
        gameController = GameObject.FindGameObjectWithTag("Player").GetComponent<GameController>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            gameController.UpadateCheckPoint(transform.position);
        }
    }
}
