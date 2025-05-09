using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2 : MonoBehaviour
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

    private int direction = 1;

    private int jumpCount = 0;
    public int maxJumps = 2;
    public bool isCrouching = false;
    public float crouchScaleY = 0.8f;
    private Vector3 originalScale;

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
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // zablokuj ruch przy kucaniu
            return;
        }

        if (grounded && jumpForce == 0.0f)
        {
            rb.linearVelocity = new Vector2(speed * gI.valueX, rb.linearVelocity.y);
        }
    }

    private void Flip()
    {
        if (gI.valueX > 0)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f); // patrz w prawo
            direction = 1;
        }
        else if (gI.valueX < 0)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f); // patrz w lewo
            direction = -1;
        }
    }





    private void PlayerJump()
    {
        if (gI.jumpInput && (grounded || jumpCount < maxJumps))
        {
            if (grounded)
            {
                jumpForce += 0.8f;
                preJump = true;
                rb.sharedMaterial = bounceMat;
            }

            // double jump logic
            if (!grounded && jumpForce == 0.0f && jumpCount < maxJumps)
            {
                rb.linearVelocity = new Vector2(gI.valueX * speed, 0); // reset pionowego ruchu
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 25f); // możesz ustawić siłę 2. skoku oddzielnie
                jumpCount++;
            }
        }
        else
        {
            preJump = false;
        }

        if ((gI.jumpInput && grounded && jumpForce >= 40.0f) || (!gI.jumpInput && jumpForce >= 0.1f))
        {
            float tempX = gI.valueX * speed;
            float tempY = jumpForce;
            rb.linearVelocity = new Vector2(tempX, tempY);
            jumpCount++;
            Invoke("ResetJump", 0.025f);
        }

        if (rb.linearVelocity.y <= -1)
        {
            rb.sharedMaterial = normalMat;
        }
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
            jumpCount = 0; // reset double jump przy lądowaniu
        }
    }

    private void HandleCrouch()
    {
        // Załóżmy, że kucanie to klawisz "S" lub "DownArrow"
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
}
