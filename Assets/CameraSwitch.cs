using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    public Transform player;

    // Rozmiar segmentu (np. 2 jednostki szerokoœci, 4 jednostki wysokoœci)
    public Vector2 segmentSize = new Vector2(2f, 4f);

    private Vector2 currentSegment; // Numer aktualnego segmentu (np. [0,0])

    void Start()
    {
       

        // Obliczamy, na którym segmencie startujemy
        currentSegment = GetSegment(player.position);

        // Ustawiamy kamerê dok³adnie na œrodek segmentu
        Vector3 newCamPos = SegmentToWorldPosition(currentSegment);
        transform.position = new Vector3(newCamPos.x, newCamPos.y, transform.position.z);
    }

    void LateUpdate()
    {
        Vector2 playerSegment = GetSegment(player.position);

        if (playerSegment != currentSegment)
        {
            // Gracz przeszed³ do innego segmentu - przesuñ kamerê
            currentSegment = playerSegment;

            Vector3 newCamPos = SegmentToWorldPosition(currentSegment);
            transform.position = new Vector3(newCamPos.x, newCamPos.y, transform.position.z);
        }
    }

    // Oblicza numer segmentu na podstawie pozycji world
    Vector2 GetSegment(Vector3 position)
    {
        int x = Mathf.FloorToInt(position.x / segmentSize.x);
        int y = Mathf.FloorToInt(position.y / segmentSize.y);
        return new Vector2(x, y);
    }

    // Przelicza numer segmentu na pozycjê w œwiecie - œrodek segmentu
    Vector3 SegmentToWorldPosition(Vector2 segment)
    {
        float x = segment.x * segmentSize.x + segmentSize.x / 2f;
        float y = segment.y * segmentSize.y + segmentSize.y / 2f;
        return new Vector3(x, y, 0);
    }
}
