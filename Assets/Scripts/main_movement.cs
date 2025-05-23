/*using UnityEngine;

public class JumpKingMovement : MonoBehaviour
{
    public float maxJumpForce = 20f;
    public float chargeRate = 20f;
    public float horizontalForce = 5f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;

    private Rigidbody2D rb;
    private float currentCharge = 0f;
    private bool isCharging = false;
    private bool grounded = false;
    private int direction = 1; // 1 = prawo, -1 = lewo

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // zmiana kierunku tylko na ziemi
        if (grounded)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
                direction = -1;
            else if (Input.GetKey(KeyCode.RightArrow))
                direction = 1;
        }

        if (grounded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isCharging = true;
                currentCharge = 0f;
            }

            if (Input.GetKey(KeyCode.Space) && isCharging)
            {
                currentCharge += chargeRate * Time.deltaTime;
                currentCharge = Mathf.Clamp(currentCharge, 0, maxJumpForce);
            }

            if (Input.GetKeyUp(KeyCode.Space) && isCharging)
            {
                Jump();
                isCharging = false;
            }
        }
    }

    void Jump()
    {
        Vector2 jumpVector = new Vector2(direction * horizontalForce, currentCharge);
        rb.linearVelocity = jumpVector;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
*/
