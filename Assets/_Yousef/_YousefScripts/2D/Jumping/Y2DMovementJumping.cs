using UnityEngine;

public class Y2DMovementJumping : MonoBehaviour
{
    [SerializeField] float speed = 2f;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] LayerMask wallLayer;
    [SerializeField] Transform wallCheck;
    [SerializeField] float wallCheckDistance = 0.1f;

    Rigidbody2D rb;
    Vector2 playerInput;
    bool jumpPressed;
    bool touchingWall;
    Animator anim;
    float fallTimer;

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

        bool rising = rb.linearVelocity.y > 0.01f;
        bool inAir = Mathf.Abs(rb.linearVelocity.y) > 0.01f;

        float horizontalSpeed = inAir ? 0f : Mathf.Abs(playerInput.x);
        anim.SetFloat("Speed", horizontalSpeed);

        touchingWall = Physics2D.Raycast(transform.position, Vector2.right * Mathf.Sign(playerInput.x), wallCheckDistance, wallLayer);

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

        anim.SetBool("isJumping", rising);

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
        {
            float normalizedTime = anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
            anim.speed = normalizedTime >= 1f ? 0f : 1f;
        }
        else
        {
            anim.speed = 1f;
        }
    }

    void FixedUpdate()
    {
        float targetX = touchingWall ? 0 : playerInput.x * speed;
        rb.linearVelocity = new Vector2(targetX, rb.linearVelocity.y);

        if (jumpPressed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpPressed = false;
        }
    }
}
