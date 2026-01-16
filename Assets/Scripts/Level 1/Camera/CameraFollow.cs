using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    public float followSpeedX = 10f;  
    public float followSpeedY = 3f;   
    public float yOffset = 1f;

    public float deadZoneHeight = 0.5f; 

    private Vector3 velocity = Vector3.zero;

    void Update()
    {
        Vector3 targetPos = transform.position;

        
        targetPos.x = Mathf.Lerp(transform.position.x, target.position.x, followSpeedX * Time.deltaTime);

      
        float diffY = (target.position.y + yOffset) - transform.position.y;
        if (Mathf.Abs(diffY) > deadZoneHeight)
        {
            
            targetPos.y = Mathf.Lerp(transform.position.y, target.position.y + yOffset, followSpeedY * Time.deltaTime);
        }

       
        targetPos.z = -10f;

        transform.position = targetPos;
    }
}
