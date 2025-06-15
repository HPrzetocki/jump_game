using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    public float followSpeedX = 10f;  // Bardzo szybkie pod¹¿anie w osi X
    public float followSpeedY = 3f;   // Wolniejsze pod¹¿anie w osi Y
    public float yOffset = 1f;

    public float deadZoneHeight = 0.5f; // Martwa strefa w pionie

    private Vector3 velocity = Vector3.zero;

    void Update()
    {
        Vector3 targetPos = transform.position;

        // W osi X kamera pod¹¿a bardzo szybko
        targetPos.x = Mathf.Lerp(transform.position.x, target.position.x, followSpeedX * Time.deltaTime);

        // W osi Y robimy martw¹ strefê - jeœli gracz jest w pewnym zakresie wokó³ kamery, kamera siê nie rusza
        float diffY = (target.position.y + yOffset) - transform.position.y;
        if (Mathf.Abs(diffY) > deadZoneHeight)
        {
            // Kamera powoli pod¹¿a w osi Y, ale nie idealnie synchronicznie
            targetPos.y = Mathf.Lerp(transform.position.y, target.position.y + yOffset, followSpeedY * Time.deltaTime);
        }

        // Kamera ma sta³¹ pozycjê w osi Z (dla 2D -10)
        targetPos.z = -10f;

        transform.position = targetPos;
    }
}
