using UnityEngine;
public class PlayerCon : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speed = 5f;
    public float maxJumpForce = 10f;
    public float minJumpForce = 5f;
    public float jumpChargeTime = 1f;
    private float jumpChargeCounter;
    private bool isChargingJump;
    private float moveInput;

    private bool isGrounded;
    public Transform feetPos;
    public float checkRadius = 0.2f;
    public LayerMask GroundKind;

    private bool isJumping;
    private float jumpDirection;
    private bool canDoubleJump;

    public float crouchSpeed = 2.5f;
    private bool isCrouching;
    public Collider2D standingCollider;
    public Collider2D crouchingCollider;

    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10f;
    public float bulletOffset = 0.5f;

    private bool isWalled;
    public LayerMask WallKind;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (jumpChargeTime <= 0f) jumpChargeTime = 1f;
    }

    private void FixedUpdate()
    {
        if (isGrounded && !isJumping && !isChargingJump)
        {
            moveInput = Input.GetAxisRaw("Horizontal");
            float currentSpeed = isCrouching ? crouchSpeed : speed;
            rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);
        }
        else if (!isGrounded)
        {
            rb.linearVelocity = new Vector2(jumpDirection * speed, rb.linearVelocity.y);
        }
        else if (isChargingJump)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    private void Update()
    {
        isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, GroundKind);

        if (isGrounded && rb.linearVelocity.y <= 0)
        {
            isJumping = false;
            canDoubleJump = true;
        }

        if (Input.GetKeyDown(KeyCode.Space) && !isChargingJump)
        {
            if (isGrounded || (!isGrounded && canDoubleJump))
            {
                isChargingJump = true;
                jumpChargeCounter = 0f;
            }
        }

        if (Input.GetKey(KeyCode.Space) && isChargingJump)
        {
            jumpChargeCounter += Time.deltaTime;
            if (jumpChargeCounter >= jumpChargeTime)
            {
                jumpChargeCounter = jumpChargeTime;
            }
        }

        if (Input.GetKeyUp(KeyCode.Space) && isChargingJump)
        {
            isChargingJump = false;
            isJumping = true;
            float jumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, jumpChargeCounter / jumpChargeTime);
            if (float.IsNaN(jumpForce) || float.IsInfinity(jumpForce))
            {
                jumpForce = minJumpForce;
            }

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpDirection = moveInput;
            if (!isGrounded) canDoubleJump = false;
        }

        if (moveInput > 0)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if (moveInput < 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = true;
            standingCollider.enabled = false;
            crouchingCollider.enabled = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            isCrouching = false;
            standingCollider.enabled = true;
            crouchingCollider.enabled = false;
        }

    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (WallKind == (WallKind | (1 << collision.gameObject.layer)))
        {
            isWalled = true;
            moveInput = -moveInput; // Zmiana kierunku ruchu
            rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y); // Dodanie impulsu
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (WallKind == (WallKind | (1 << collision.gameObject.layer)))
        {
            isWalled = false;
        }
    }
}
