using UnityEngine;

public class JumpKingCamera : MonoBehaviour
{
    public Transform player;               // Referencja do gracza
    public Vector2 deadZoneSize = new Vector2(4f, 3f); // Rozmiar strefy martwej (w jednostkach œwiata)

    private Vector3 camPosition;

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("JumpKingCamera: nie przypisano gracza!");
            enabled = false;
            return;
        }
        // Kamera zaczyna nad graczem
        camPosition = new Vector3(player.position.x, player.position.y, transform.position.z);
        transform.position = camPosition;
    }

    void LateUpdate()
    {
        float leftBound = camPosition.x - deadZoneSize.x / 2f;
        float rightBound = camPosition.x + deadZoneSize.x / 2f;
        float bottomBound = camPosition.y - deadZoneSize.y / 2f;
        float topBound = camPosition.y + deadZoneSize.y / 2f;

        bool moved = false;

        if (player.position.x < leftBound)
        {
            camPosition.x -= deadZoneSize.x;
            moved = true;
        }
        else if (player.position.x > rightBound)
        {
            camPosition.x += deadZoneSize.x;
            moved = true;
        }

        if (player.position.y < bottomBound)
        {
            camPosition.y -= deadZoneSize.y;
            moved = true;
        }
        else if (player.position.y > topBound)
        {
            camPosition.y += deadZoneSize.y;
            moved = true;
        }

        if (moved)
        {
            transform.position = new Vector3(camPosition.x, camPosition.y, transform.position.z);
        }
    }
}


