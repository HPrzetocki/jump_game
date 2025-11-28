using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Analityka
using Unity.Services.Analytics;
using Unity.Services.Core;


public class PlayerController2 : MonoBehaviour
{
    [SerializeField] private PlayerMovmentState playerMovmentState;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioClip jumpSound;
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

    public float horizontalJumpBoost = 1.5f;
    public float verticalJumpScale = 0.6f;

    private float maxJumpForce = 90f;

    // Do analityki / debugów
    private bool lastCrouchInput = false;
    private bool wasGrounded = false;
    private bool firstStatusCheckDone = false;

    // Helper: czy Unity Services są zainicjalizowane
    private bool IsAnalyticsReady =>
        UnityServices.State == ServicesInitializationState.Initialized;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gI = GetComponent<GatherInput>();
        originalScale = transform.localScale;

        Debug.Log("[Player] PlayerController2 Start");
    }

    private void FixedUpdate()
    {
        Flip();
        PlayerJump();
        CheckStatus();
        PlayerMove();
        HandleCrouch();
        /* HandleAnimations(); */
    }

    /*
    private void HandleAnimations()
    {
        if (Mathf.Abs(gI.valueX) > 0.1f && grounded && !isCrouching)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }
    */

    private void PlayerMove()
    {
        if (isCrouching)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }

        if (grounded)
        {
            if (preJump)
            {
                // Zatrzymaj ruch poziomy podczas ładowania skoku
                rb.velocity = new Vector2(0, rb.velocity.y);
                playerMovmentState.SetMoveState(PlayerMovmentState.MoveState.Crouch);
            }
            else
            {
                rb.velocity = new Vector2(speed * gI.valueX, rb.velocity.y);
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
        // Ładowany skok z ziemi
        if (gI.jumpInput && grounded)
        {
            if (!preJump)
            {
                Debug.Log("[Player] Zaczynam ładować skok z ziemi");
            }

            preJump = true;
            rb.sharedMaterial = bounceMat;

            jumpForce += 2.5f;
            if (jumpForce >= maxJumpForce)
            {
                jumpForce = maxJumpForce;
                Debug.Log("[Player] Osiągnięto maxJumpForce, wykonuję skok z ziemi");
                PerformJump(false); // skok z ziemi
            }
        }

        // Zakończenie ładowania i wykonanie skoku
        if (!gI.jumpInput && preJump && grounded && jumpForce > 0f)
        {
            Debug.Log("[Player] Zwolniono przycisk skoku, wykonuję skok z ziemi");
            PerformJump(false); // skok z ziemi
        }

        // Podwójny skok w powietrzu


        if (rb.velocity.y <= -1)
        {
            rb.sharedMaterial = normalMat;
        }
    }

    private void PerformJump(bool isAirJump)
    {
        float horizontalInput = gI.valueX;

        if (Mathf.Abs(horizontalInput) < 0.1f)
        {
            horizontalInput = direction;
        }

        float tempX = horizontalInput * speed * horizontalJumpBoost;
        float tempY = jumpForce * verticalJumpScale;

        rb.velocity = new Vector2(tempX, tempY);

        jumpCount++;
        preJump = false;
        rb.sharedMaterial = normalMat;
        playerMovmentState.SetMoveState(PlayerMovmentState.MoveState.Jump);

        Debug.Log($"[Player] Skok z ziemi: vel=({tempX}, {tempY}), jumpCount={jumpCount}");
        SoundFXManager.instance.PlaySoundFXClip(jumpSound, transform, 1f);

        // Analityka – skok z ziemi
        SendJumpAnalytics(isAirJump, tempX, tempY);

        ResetJump();
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

        // Pierwsze wywołanie – inicjalizacja, bez eventu lądowania
        if (!firstStatusCheckDone)
        {
            wasGrounded = grounded;
            firstStatusCheckDone = true;
        }
        else
        {
            // Lądowanie – event tylko przy przejściu z powietrza na ziemię
            if (grounded && !wasGrounded)
            {
                Debug.Log("[Player] Lądowanie na ziemi");
                SendLandAnalytics();
                jumpCount = 0;
            }
        }

        if (grounded)
        {
            jumpCount = 0;
        }

        wasGrounded = grounded;
    }

    private void HandleCrouch()
    {
        bool crouchInput = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);

        // Zmiana stanu kucania – tylko raz przy zmianie
        if (crouchInput != lastCrouchInput)
        {
            Debug.Log("[Player] Zmiana stanu kucania: " + crouchInput);
            SendCrouchAnalytics(crouchInput);
            lastCrouchInput = crouchInput;
        }

        if (crouchInput)
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
        Debug.Log("[Player] ResetPlayerState");
        jumpForce = 0f;
        preJump = false;
        jumpCount = 0;
    }

    // ==========================
    // ===== METODY ANALITYKI ===
    // ==========================

    private void SendJumpAnalytics(bool isAirJump, float velX, float velY)
    {
        if (!IsAnalyticsReady)
        {
            Debug.LogWarning($"[Analytics] Jump event SKIPPED - services not ready. isAirJump={isAirJump}");
            return;
        }

        try
        {
            var jumpEvent = new CustomEvent("player_jump")
            {
                { "is_air_jump", isAirJump },
                { "velocity_x", velX },
                { "velocity_y", velY },
                { "position_x", transform.position.x },
                { "position_y", transform.position.y },
                { "jump_count", jumpCount },
                { "grounded_before_jump", grounded }
            };

            AnalyticsService.Instance.RecordEvent(jumpEvent);

            Debug.Log(
                $"[Analytics] Sent player_jump | air={isAirJump}, vel=({velX}, {velY}), " +
                $"pos=({transform.position.x}, {transform.position.y}), jumpCount={jumpCount}"
            );
        }
        catch (System.Exception e)
        {
            Debug.LogError("[Analytics] Failed to send player_jump: " + e);
        }
    }

    private void SendCrouchAnalytics(bool started)
    {
        if (!IsAnalyticsReady)
        {
            Debug.LogWarning($"[Analytics] Crouch event SKIPPED - services not ready. started={started}");
            return;
        }

        try
        {
            var crouchEvent = new CustomEvent("player_crouch")
            {
                { "started", started },
                { "position_x", transform.position.x },
                { "position_y", transform.position.y }
            };

            AnalyticsService.Instance.RecordEvent(crouchEvent);

            Debug.Log(
                $"[Analytics] Sent player_crouch | started={started}, " +
                $"pos=({transform.position.x}, {transform.position.y})"
            );
        }
        catch (System.Exception e)
        {
            Debug.LogError("[Analytics] Failed to send player_crouch: " + e);
        }
    }

    private void SendLandAnalytics()
    {
        if (!IsAnalyticsReady)
        {
            Debug.LogWarning("[Analytics] Land event SKIPPED - services not ready.");
            return;
        }

        try
        {
            var landEvent = new CustomEvent("player_land")
            {
                { "position_x", transform.position.x },
                { "position_y", transform.position.y },
                { "landing_velocity_y", rb.velocity.y }
            };

            AnalyticsService.Instance.RecordEvent(landEvent);

            Debug.Log(
                $"[Analytics] Sent player_land | pos=({transform.position.x}, {transform.position.y}), " +
                $"velY={rb.velocity.y}"
            );
        }
        catch (System.Exception e)
        {
            Debug.LogError("[Analytics] Failed to send player_land: " + e);
        }
    }
}
