using UnityEngine;

public class Y2DMovementFinalNoJump : MonoBehaviour
{
    [SerializeField] float speed = 2f;
    [SerializeField] SpriteRenderer spriteRenderer;

    Rigidbody2D rb;
    Vector2 playerInput;
    Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        playerInput.x = Input.GetAxisRaw("Horizontal");

        if (playerInput.x > 0) spriteRenderer.flipX = false;
        else if (playerInput.x < 0) spriteRenderer.flipX = true;

        if (anim != null)
            anim.SetFloat("Speed", Mathf.Abs(playerInput.x));
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(playerInput.x * speed, rb.linearVelocity.y);
    }

    public void AddSpeed(float extra)
    {
        speed += extra;
    }

    public void AddGravityScale(float extra)
    {
        rb.gravityScale += extra;
    }
}
