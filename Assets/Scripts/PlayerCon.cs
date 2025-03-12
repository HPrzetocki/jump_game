using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerCon : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speed;
    public float jumpForce;
    private float moveInput;

    private bool isGrounded;
    public Transform feetPos;
    public float checkRadius;
    public LayerMask GroundKind;

    private float jumpTimeCouinter;
    public float jumpTime;
    private bool isJumping;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);
    }
    private void Update()
    {
        isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, GroundKind);

        if (moveInput > 0)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if (moveInput < 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }

        if (isGrounded == true && Input.GetKeyDown(KeyCode.Space)){
            isJumping = true;
            jumpTimeCouinter = jumpTime;
            rb.linearVelocity = Vector2.up * jumpForce;
        }
        if (Input.GetKey(KeyCode.Space) && isJumping == true)
        {
            if (jumpTimeCouinter > 0)
            {
                rb.linearVelocity = Vector2.up * jumpForce;
                jumpTimeCouinter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
                
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                isJumping = false;
            }
        }
    }
