using UnityEngine;

public class Y2DMovement : MonoBehaviour
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

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {

        playerInput.x = Input.GetAxisRaw("Horizontal");
        if (playerInput.x > 0) spriteRenderer.flipX = false;
        else if (playerInput.x < 0) spriteRenderer.flipX = true;
        anim.SetFloat("Speed", Mathf.Abs(playerInput.x));

        touchingWall = Physics2D.Raycast(transform.position, Vector2.right * Mathf.Sign(playerInput.x), wallCheckDistance, wallLayer);

        if (Input.GetButtonDown("Jump") && Mathf.Abs(rb.linearVelocity.y) < 0.01f)
        {
            jumpPressed = true;
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
