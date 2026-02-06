using UnityEngine;

public class Y2DMovementFinal : MonoBehaviour
{
    [SerializeField] float speed = 2f;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] AudioClip jumpSound;
    [SerializeField] float volume = 1f;

    Rigidbody2D rb;
    Vector2 playerInput;
    bool jumpPressed;
    Animator anim;
    float fallTimer;

    bool canPlayJumpSound = true;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        fallTimer = 0f;
    }

    void Update()
    {
        playerInput.x = Input.GetAxisRaw("Horizontal");

        if (playerInput.x > 0) spriteRenderer.flipX = false;
        else if (playerInput.x < 0) spriteRenderer.flipX = true;

        bool inAir = Mathf.Abs(rb.linearVelocity.y) > 0.01f;

        anim.SetFloat("Speed", Mathf.Abs(playerInput.x));

        if (Input.GetButtonDown("Jump") && Mathf.Abs(rb.linearVelocity.y) < 0.01f)
            jumpPressed = true;

        if (rb.linearVelocity.y < 0f)
        {
            fallTimer += Time.deltaTime;
            if (fallTimer >= 0.05f) anim.SetBool("isFalling", true);
        }
        else
        {
            fallTimer = 0f;
            anim.SetBool("isFalling", false);
        }

        anim.SetBool("isJumping", inAir);

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
        {
            float t = anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
            anim.speed = t >= 1f ? 0f : 1f;
        }
        else
        {
            anim.speed = 1f;
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(playerInput.x * speed, rb.linearVelocity.y);

        if (jumpPressed)
        {
            if (canPlayJumpSound && jumpSound != null)
                AudioSource.PlayClipAtPoint(jumpSound, transform.position, volume);

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpPressed = false;
        }
    }

    public void AddSpeed(float extra)
    {
        speed += extra;
    }

    public void AddJumpForce(float extra)
    {
        jumpForce += extra;

        if (jumpForce <= 0f)
            canPlayJumpSound = false;
        else
            canPlayJumpSound = true;
    }

    public void AddGravityScale(float extra)
    {
        rb.gravityScale += extra;
    }
}