using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController55 : MonoBehaviour
{
    public float speed;
    public float jumpForce;

    private Rigidbody2D rb;
    private GatherInput gI;

    public float rayLength;
    public bool grounded;
    public bool preJump = false;
    public LayerMask groundLayer;
    public Transform checkPointLeft;
    public Transform checkPointRight;
    public PhysicsMaterial2D bounceMat, normalMat;
    public float doubleJumpForceMultiplier = 0.8f; // 80% siły pierwszego skoku

    private int direction = 1;

    private int jumpCount = 0;
    public int maxJumps = 2;
    public bool isCrouching = false;
    public float crouchScaleY = 0.8f;
    private Vector3 originalScale;

    public float horizontalJumpBoost = 1.5f;
    public float verticalJumpScale = 0.6f;

    private float maxJumpForce = 90f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gI = GetComponent<GatherInput>();
        originalScale = transform.localScale;
    }

    private void FixedUpdate()
    {
        Flip();
        PlayerJump();
        CheckStatus();
        PlayerMove();
        HandleCrouch();
    }

    private void PlayerMove()
    {
        if (isCrouching)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        if (grounded)
        {
            if (preJump)
            {
                // Zatrzymaj ruch poziomy podczas ładowania skoku
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }
            else
            {
                rb.linearVelocity = new Vector2(speed * gI.valueX, rb.linearVelocity.y);
            }
        }
    }

    private void Flip()
    {
        if (gI.valueX > 0)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            direction = 1;
        }
        else if (gI.valueX < 0)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            direction = -1;
        }
    }

    private void PlayerJump()
    {
        if (gI.jumpInput && grounded)
        {
            preJump = true;
            rb.sharedMaterial = bounceMat;

            jumpForce += 2.5f;
            if (jumpForce >= maxJumpForce)
            {
                jumpForce = maxJumpForce;
                PerformJump();
            }
        }

        if (!gI.jumpInput && preJump && grounded && jumpForce > 0f)
        {
            PerformJump();
        }

if (gI.jumpInput && !grounded && jumpCount < maxJumps && jumpForce == 0.0f)
{
    float horizontalInput = gI.valueX;

    if (Mathf.Abs(horizontalInput) < 0.1f)
    {
        horizontalInput = direction;
    }

    float tempX = horizontalInput * speed * horizontalJumpBoost;
    float tempY = maxJumpForce * verticalJumpScale * doubleJumpForceMultiplier;

    // Zamiast ustawiać velocity, dodajemy siłę – to daje naturalny efekt
    rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // wyzeruj pionowość, by impuls zadziałał czysto w górę
    rb.AddForce(new Vector2(tempX, tempY), ForceMode2D.Impulse);

    jumpCount++;
}


        if (rb.linearVelocity.y <= -1)
        {
            rb.sharedMaterial = normalMat;
        }
    }

    private void PerformJump()
    {
        float horizontalInput = gI.valueX;

        if (Mathf.Abs(horizontalInput) < 0.1f)
        {
            horizontalInput = direction;
        }

        float tempX = horizontalInput * speed * horizontalJumpBoost;
        float tempY = jumpForce * verticalJumpScale;

        rb.linearVelocity = new Vector2(tempX, tempY);

        jumpCount++;
        ResetJump();
        preJump = false;

        rb.sharedMaterial = normalMat;
    }

    private void ResetJump()
    {
        jumpForce = 0.0f;
    }

    private void CheckStatus()
    {
        RaycastHit2D leftCheckHit = Physics2D.Raycast(checkPointLeft.position, Vector2.down, rayLength, groundLayer);
        RaycastHit2D rightCheckHit = Physics2D.Raycast(checkPointRight.position, Vector2.down, rayLength, groundLayer);

        grounded = leftCheckHit.collider != null || rightCheckHit.collider != null;

        if (grounded)
        {
            jumpCount = 0;
        }
    }

    private void HandleCrouch()
    {
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            isCrouching = true;
            transform.localScale = new Vector3(originalScale.x, crouchScaleY, originalScale.z);
        }
        else
        {
            isCrouching = false;
            transform.localScale = originalScale;
        }
    }

    public void ResetPlayerState()
    {
        jumpForce = 0f;
        preJump = false;
        jumpCount = 0;
    }
}