using UnityEngine;

public class PlayerFootsteps2D : MonoBehaviour
{
    [Header("Dźwięki kroków")]
    [SerializeField] private AudioClip[] footstepClips;
    [SerializeField] private float stepInterval = 0.4f; // co ile sekund krok
    [SerializeField] private float minSpeed = 0.1f;     // od jakiej prędkości liczymy krok

    [Header("Sprawdzanie ziemi")]
    [SerializeField] private Transform groundCheck;     // punkt pod stopami
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundMask;      // warstwa podłoża

    private Rigidbody2D rb;
    private float stepTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
            Debug.LogError("PlayerFootsteps2D: Brak Rigidbody2D na obiekcie gracza!");
    }

    private void Update()
    {
        if (rb == null) return;

        // ruch poziomy (X) – w 2D na ogół tylko to
        bool isMoving = Mathf.Abs(rb.velocity.x) > minSpeed;

        bool isGrounded = false;

        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundMask);

            // debug – w Scene widać kółko
            Debug.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckRadius, isGrounded ? Color.green : Color.red);
        }
        else
        {
            Debug.LogWarning("PlayerFootsteps2D: groundCheck nie jest ustawiony!");
        }

        if (isMoving && isGrounded)
        {
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0f)
            {
                PlayFootstep();
                stepTimer = stepInterval;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }

    private void PlayFootstep()
    {
        if (footstepClips == null || footstepClips.Length == 0)
        {
            Debug.LogWarning("PlayerFootsteps2D: brak przypisanych footstepClips!");
            return;
        }

        if (SoundFXManager.instance == null)
        {
            Debug.LogError("PlayerFootsteps2D: Brak SoundFXManager.instance w scenie!");
            return;
        }

        AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];

        Debug.Log("FOOTSTEP: " + clip.name); // zobaczysz w konsoli kiedy gra krok

        SoundFXManager.instance.PlaySoundFXClip(clip, transform, 1f);
    }

    // żeby w edytorze było widać zasięg sprawdzania ziemi
    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
