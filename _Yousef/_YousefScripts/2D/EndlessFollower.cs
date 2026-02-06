using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class EndlessFollower : MonoBehaviour
{
    [SerializeField] float speed = 3f;
    [SerializeField] float maxSpeed = 10f;
    [SerializeField] float timeToMaxSpeed = 1f;
    [SerializeField] float followYOffset = 0f;
    [SerializeField] bool keepZ = true;
    [SerializeField] bool facePlayer = true;

    [SerializeField] float bobAmplitude = 0.25f;
    [SerializeField] float bobFrequency = 1.5f;
    [SerializeField] float lateralWander = 0.2f;
    [SerializeField] float lateralFrequency = 0.6f;

    [SerializeField] bool dashEnabled = true;
    [SerializeField] float dashCooldown = 2.5f;
    [SerializeField] float dashDistance = 3f;
    [SerializeField] float dashSpeed = 12f;
    [SerializeField] bool passThroughDuringDash = true;

    [SerializeField] float exitDistance = 1.2f;
    [SerializeField] float exitDuration = 1f;

    [SerializeField] float destroyDelayWhenPlayerGone = 2.0001f;

    Transform player;
    Collider2D playerCollider;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rb;
    Collider2D col;
    float initialZ;

    bool destroying;
    bool isDashing;
    float dashTimer;

    bool exitingPlayer;
    Vector2 exitStart;
    Vector2 exitTarget;
    float exitTimer;

    float currentSpeed;
    float acceleration;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p == null)
        {
            Destroy(gameObject);
            return;
        }

        player = p.transform;
        playerCollider = p.GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        initialZ = transform.position.z;

        rb.bodyType = RigidbodyType2D.Kinematic;

        currentSpeed = speed; // start at serialized speed
        acceleration = (maxSpeed - speed) / Mathf.Max(0.01f, timeToMaxSpeed);
    }

    void Update()
    {
        if (player == null) return;

        if (playerCollider == null || !playerCollider.enabled)
        {
            if (!destroying)
                StartCoroutine(DestroyAfterDelay());
            return;
        }

        if (dashEnabled && !isDashing && !exitingPlayer)
        {
            dashTimer += Time.deltaTime;
            if (dashTimer >= dashCooldown)
                StartCoroutine(DoDash());
        }

        if (facePlayer && spriteRenderer != null)
            spriteRenderer.flipX = player.position.x < transform.position.x;
    }

    void FixedUpdate()
    {
        if (player == null) return;

        currentSpeed = Mathf.MoveTowards(
            currentSpeed,
            maxSpeed,
            acceleration * Time.fixedDeltaTime
        );

        if (exitingPlayer)
        {
            exitTimer += Time.fixedDeltaTime;
            float t = Mathf.Clamp01(exitTimer / exitDuration);
            rb.MovePosition(Vector2.Lerp(exitStart, exitTarget, t));
            if (t >= 1f) exitingPlayer = false;
            return;
        }

        if (isDashing) return;

        Vector3 target = player.position;
        target.y += followYOffset;
        target.y += Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
        target.x += Mathf.Sin(Time.time * lateralFrequency) * lateralWander;
        if (keepZ) target.z = initialZ;

        rb.MovePosition(
            Vector2.MoveTowards(
                rb.position,
                (Vector2)target,
                currentSpeed * Time.fixedDeltaTime
            )
        );
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (exitingPlayer || isDashing) return;
        BeginExit(other.transform);
    }

    void BeginExit(Transform playerTransform)
    {
        exitingPlayer = true;
        exitTimer = 0f;
        exitStart = rb.position;

        Vector2 dir = (rb.position - (Vector2)playerTransform.position).normalized;
        if (dir == Vector2.zero) dir = Vector2.right;

        exitTarget = exitStart + dir * exitDistance;
    }

    IEnumerator DoDash()
    {
        isDashing = true;
        dashTimer = 0f;

        Vector2 start = rb.position;
        Vector2 dir = ((Vector2)player.position - start).normalized;
        if (dir == Vector2.zero) dir = Vector2.right;

        Vector2 end = start + dir * dashDistance;
        float duration = Mathf.Max(0.01f, Vector2.Distance(start, end) / dashSpeed);
        float t = 0f;

        bool originalTrigger = col.isTrigger;
        if (passThroughDuringDash) col.isTrigger = true;

        while (t < duration)
        {
            t += Time.fixedDeltaTime;
            rb.MovePosition(Vector2.Lerp(start, end, t / duration));
            yield return new WaitForFixedUpdate();
        }

        rb.MovePosition(end);
        if (passThroughDuringDash) col.isTrigger = originalTrigger;

        yield return new WaitForSeconds(0.12f);
        isDashing = false;
    }

    IEnumerator DestroyAfterDelay()
    {
        destroying = true;
        yield return new WaitForSeconds(destroyDelayWhenPlayerGone);
        Destroy(gameObject);
    }
}
