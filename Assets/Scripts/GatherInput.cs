using UnityEngine;

public class GatherInput : MonoBehaviour
{
    public float valueX;
    public bool jumpInput;

    void Update()
    {
        valueX = Input.GetAxisRaw("Horizontal");
        jumpInput = Input.GetButton("Jump");
    }
}
