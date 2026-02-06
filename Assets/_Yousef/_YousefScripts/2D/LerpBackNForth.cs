using UnityEngine;

public class RidePlatformAbsolute : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float returnSpeed = 10f;
    [SerializeField] private float cooldown = 1f;
    [SerializeField] private float targetX = 122f;
    [SerializeField] private float autoReturnDelay = 6f;

    private Vector3 startPos;
    private Vector3 targetPos;
    private Vector3 currentTarget;

    private bool moving = false;
    private bool goingToTarget = true;
    private float cooldownTimer = 0f;
    private float autoReturnTimer = 0f;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        startPos = transform.position;
        targetPos = new Vector3(targetX, startPos.y, startPos.z);
        currentTarget = startPos;

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
            return;
        }

        if (!moving)
        {
            if (!goingToTarget && currentTarget == targetPos)
            {
                autoReturnTimer += Time.deltaTime;

                if (autoReturnTimer >= autoReturnDelay)
                {
                    currentTarget = startPos;
                    moving = true;
                    autoReturnTimer = 0f;
                }
            }

            return;
        }

        Vector3 previousPos = transform.position;
        float currentSpeed = (currentTarget == startPos) ? returnSpeed : speed;
        transform.position = Vector3.MoveTowards(transform.position, currentTarget, currentSpeed * Time.deltaTime);

        if (spriteRenderer != null)
        {
            float dx = transform.position.x - previousPos.x;
            if (Mathf.Abs(dx) > 0.0001f)
                spriteRenderer.flipX = dx > 0f;
        }

        if (Vector3.Distance(transform.position, currentTarget) < 0.0001f)
        {
            transform.position = currentTarget;
            moving = false;
            cooldownTimer = cooldown;
            autoReturnTimer = 0f;
            goingToTarget = (currentTarget == startPos);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (moving || cooldownTimer > 0f) return;

        if (goingToTarget)
        {
            currentTarget = targetPos;
            moving = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (moving || cooldownTimer > 0f) return;

        if (!goingToTarget)
        {
            currentTarget = startPos;
            moving = true;
        }
    }
}