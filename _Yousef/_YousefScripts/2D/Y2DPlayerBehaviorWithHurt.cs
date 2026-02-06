using UnityEngine;
using Unity.Cinemachine;

public class Y2DPlayerBehaviorWithHurt : MonoBehaviour
{
    [SerializeField] int playerHealth = 2;
    [SerializeField] int damageTaken = 1;

    [Header("Hurt")]
    [SerializeField] float hurtDuration = 2f;
    [SerializeField] float invincibleDuration = 2f;

    [Header("Hit Red Flash")]
    [SerializeField] Color hitColor = Color.red;
    [SerializeField] float hitRedDuration = 0.05f;

    [Header("Invincibility Flicker")]
    [SerializeField] float flickerAlphaA = 0.3f;
    [SerializeField] float flickerAlphaB = 0.7f;
    [SerializeField] float flickerInterval = 0.15f;

    [Header("Low Health")]
    [SerializeField] Color lowHealthColor = new Color(1f, 0.4f, 0.4f);
    [SerializeField] Color lowHealthColorAfterTrigger = new Color(1f, 0.2f, 0.2f);

    [Header("Knockback")]
    [SerializeField] float knockbackDistanceX = 0.4f;
    [SerializeField] float knockbackDistanceY = 0.1f;

    [Header("Death")]
    [SerializeField] float deathYOffset = 0.2f;
    [SerializeField] float deathYOffsetDelay = 0.5f;

    [Header("Respawn")]
    [SerializeField] float respawnDelay = 2f;
    [SerializeField] Vector3 currentCheckpoint = new Vector3(0.22f, 4.08f, 0f);

    [Header("Fall Death")]
    [SerializeField] float stopFollowingY = -5f;
    [SerializeField] float deathY = -10f;

    [Header("Camera")]
    [SerializeField] CinemachineCamera virtualCamera;

    [Header("Audio")]
    [SerializeField] AudioClip damageSound;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip realDeathSound;

    [SerializeField] SpriteRenderer spriteRenderer;

    int currentHealth;
    Animator anim;
    Rigidbody2D rb;
    Collider2D col;
    MonoBehaviour movement;

    bool isDead;
    bool isHurt;
    bool isInvincible;
    bool yOffsetApplied;
    bool cameraStopped;

    bool lockedFlipX;
    float lockedScaleX;

    Transform originalCameraFollow;
    Color baseColor;
    Color permanentColor;

    bool onlyEvilEnemyDamage = false;

    public int CurrentHealth { get => currentHealth; set => currentHealth = value; }
    public SpriteRenderer SpriteRenderer => spriteRenderer;

    void Start()
    {
        currentHealth = playerHealth;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        movement = GetComponent<Y2DMovementFinal>();
        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
        baseColor = spriteRenderer != null ? spriteRenderer.color : Color.white;
        permanentColor = baseColor;
        if (virtualCamera != null)
            originalCameraFollow = virtualCamera.Follow;
        UpdateHealthColor();
    }

    void Update()
    {
        if (!cameraStopped && transform.position.y <= stopFollowingY)
        {
            if (virtualCamera != null)
                virtualCamera.Follow = null;
            cameraStopped = true;
        }

        if (!isDead && transform.position.y < deathY)
            FallDeath();

        if (virtualCamera != null && virtualCamera.Follow == null && !isDead && cameraStopped == false)
            virtualCamera.Follow = originalCameraFollow;
    }

    public void SetPermanentColor(Color c)
    {
        permanentColor = c;
        if (spriteRenderer != null)
            spriteRenderer.color = permanentColor;
    }

    public void SetOnlyEvilEnemyDamage(bool value)
    {
        onlyEvilEnemyDamage = value;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead || isInvincible) return;

        if (other.CompareTag("EvilEnemy"))
        {
            if (currentHealth - damageTaken <= 0)
                RealDie();
            else
            {
                if (damageSound != null)
                    AudioSource.PlayClipAtPoint(damageSound, transform.position);
                TakeDamage(damageTaken, other.transform);
            }
        }
        else if (other.CompareTag("Enemy") || other.CompareTag("Obstacles"))
        {
            if (!onlyEvilEnemyDamage)
            {
                if (damageSound != null)
                    AudioSource.PlayClipAtPoint(damageSound, transform.position);
                TakeDamage(damageTaken, other.transform);
            }
        }
    }

    void TakeDamage(int amount, Transform source)
    {
        if (isDead || isInvincible) return;
        currentHealth -= amount;
        UpdateHealthColor();

        if (currentHealth <= 0)
        {
            Die();
            return;
        }
        ApplyKnockback(source);
        StartCoroutine(HitRedThenFlicker());
        StartCoroutine(HurtRoutine());
    }

    void ApplyKnockback(Transform source)
    {
        float dir = transform.position.x < source.position.x ? -1f : 1f;
        transform.position = new Vector3(
            transform.position.x + dir * knockbackDistanceX,
            transform.position.y + knockbackDistanceY,
            transform.position.z
        );
    }

    System.Collections.IEnumerator HitRedThenFlicker()
    {
        isInvincible = true;
        if (spriteRenderer != null)
            spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(hitRedDuration);

        float timer = 0f;
        bool toggle = false;
        while (timer < invincibleDuration)
        {
            if (spriteRenderer != null)
            {
                Color c = spriteRenderer.color;
                c.a = toggle ? flickerAlphaA : flickerAlphaB;
                spriteRenderer.color = c;
            }
            toggle = !toggle;
            timer += flickerInterval;
            yield return new WaitForSeconds(flickerInterval);
        }

        UpdateHealthColor();
        isInvincible = false;
    }

    System.Collections.IEnumerator HurtRoutine()
    {
        isHurt = true;
        if (anim != null) anim.SetBool("isHurt", true);
        yield return new WaitForSeconds(hurtDuration);
        if (anim != null) anim.SetBool("isHurt", false);
        isHurt = false;
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        if (deathSound != null)
            AudioSource.PlayClipAtPoint(deathSound, transform.position);

        if (anim != null)
        {
            anim.SetTrigger("isDead");
            anim.SetBool("isAlive", false);
        }

        if (movement) movement.enabled = false;
        if (rb) rb.simulated = false;
        if (col) col.enabled = false;

        if (spriteRenderer) lockedFlipX = spriteRenderer.flipX;
        lockedScaleX = transform.localScale.x;

        if (virtualCamera != null)
            virtualCamera.Follow = null;

        cameraStopped = true;

        StartCoroutine(DeathYOffset());
        StartCoroutine(RespawnAtCheckpoint());
    }

    void RealDie()
    {
        if (isDead) return;
        isDead = true;

        if (realDeathSound != null)
            AudioSource.PlayClipAtPoint(realDeathSound, transform.position);

        if (anim != null)
        {
            anim.SetTrigger("realDeath");
            anim.SetBool("isAlive", false);
        }

        if (movement) movement.enabled = false;
        if (rb) rb.simulated = false;
        if (col) col.enabled = false;

        if (spriteRenderer) lockedFlipX = spriteRenderer.flipX;
        lockedScaleX = transform.localScale.x;

        if (virtualCamera != null)
            virtualCamera.Follow = null;

        cameraStopped = true;

        StartCoroutine(DeathYOffset());
        StartCoroutine(RespawnAtCheckpoint());
    }

    void FallDeath()
    {
        if (isDead) return;
        isDead = true;

        if (deathSound != null)
            AudioSource.PlayClipAtPoint(deathSound, transform.position);

        if (movement) movement.enabled = false;
        if (rb) rb.simulated = false;
        if (col) col.enabled = false;

        if (spriteRenderer) lockedFlipX = spriteRenderer.flipX;
        lockedScaleX = transform.localScale.x;

        if (virtualCamera != null)
            virtualCamera.Follow = null;

        cameraStopped = true;

        StartCoroutine(DeathYOffset());
        StartCoroutine(RespawnAtCheckpoint());
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

    System.Collections.IEnumerator RespawnAtCheckpoint()
    {
        yield return new WaitForSeconds(respawnDelay);

        currentHealth = playerHealth;
        isDead = false;
        yOffsetApplied = false;
        cameraStopped = false;

        transform.position = currentCheckpoint;

        if (movement) movement.enabled = true;
        if (rb)
        {
            rb.simulated = true;
            rb.linearVelocity = Vector2.zero;
        }
        if (col) col.enabled = true;

        if (anim != null)
        {
            anim.ResetTrigger("isDead");
            anim.ResetTrigger("realDeath");
            anim.SetBool("isHurt", false);
            anim.SetBool("isAlive", true);
        }

        if (virtualCamera != null)
            virtualCamera.Follow = originalCameraFollow;

        UpdateHealthColor();
    }

    void LateUpdate()
    {
        if (!isDead) return;

        if (spriteRenderer && spriteRenderer.flipX != lockedFlipX)
            spriteRenderer.flipX = lockedFlipX;

        Vector3 s = transform.localScale;
        if (s.x != lockedScaleX)
            transform.localScale = new Vector3(lockedScaleX, s.y, s.z);
    }

    void UpdateHealthColor()
    {
        if (spriteRenderer == null) return;

        if (currentHealth == 1)
        {
            if (onlyEvilEnemyDamage)
                spriteRenderer.color = lowHealthColorAfterTrigger;
            else
                spriteRenderer.color = lowHealthColor;
        }
        else
        {
            spriteRenderer.color = permanentColor;
        }
    }

    public void Bounce(float bounceForce)
    {
        if (rb == null) return;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);
    }

    public void SetCheckpoint(Vector3 newCheckpoint)
    {
        currentCheckpoint = newCheckpoint;
    }
}