using UnityEngine;

public class Y2DPlayerBehavior : MonoBehaviour
{
    [SerializeField] int playerHealth = 3;
    [SerializeField] int damageTaken = 1;
    [SerializeField] float deathYOffset = 0.2f;
    [SerializeField] float deathYOffsetDelay = 0.5f;
    [SerializeField] SpriteRenderer spriteRenderer;

    int currentHealth;
    Animator anim;
    Rigidbody2D rb;
    Collider2D col;
    MonoBehaviour movement;
    bool yOffsetApplied;
    bool isDead;
    bool lockedFlipX;
    float lockedScaleX;

    void Start()
    {
        currentHealth = playerHealth;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        movement = GetComponent<Y2DMovementJumping>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Obstacles"))
            TakeDamage(damageTaken);
    }

    void TakeDamage(int amount)
    {
        if (currentHealth <= 0) return;

        currentHealth -= amount;
        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        anim.SetTrigger("isDead");
        if (movement) movement.enabled = false;
        if (rb) rb.simulated = false;
        if (col) col.enabled = false;
        if (spriteRenderer) lockedFlipX = spriteRenderer.flipX;
        lockedScaleX = transform.localScale.x;
        StartCoroutine(DeathYOffset());
    }

    System.Collections.IEnumerator DeathYOffset()
    {
        yield return new WaitForSeconds(deathYOffsetDelay);
        if (yOffsetApplied) yield break;
        transform.position = new Vector3(
            transform.position.x,
            transform.position.y - deathYOffset,
            transform.position.z
        );
        yOffsetApplied = true;
    }

    void LateUpdate()
    {
        if (!isDead) return;
        if (spriteRenderer && spriteRenderer.flipX != lockedFlipX) spriteRenderer.flipX = lockedFlipX;
        Vector3 s = transform.localScale;
        if (s.x != lockedScaleX) transform.localScale = new Vector3(lockedScaleX, s.y, s.z);
    }
}
