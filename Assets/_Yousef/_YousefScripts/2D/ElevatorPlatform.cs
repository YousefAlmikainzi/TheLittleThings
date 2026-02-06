using UnityEngine;

public class ElevatorPlatformAbsolute : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float cooldown = 1f;
    [SerializeField] private float targetY = 10f;
    [SerializeField] private float autoReturnDelay = 6f;

    private Vector3 startPos;
    private Vector3 targetPos;
    private Vector3 currentTarget;

    private bool moving = false;
    private bool atTop = false;

    private float cooldownTimer = 0f;
    private float autoReturnTimer = 0f;

    void Start()
    {
        startPos = transform.position;
        targetPos = new Vector3(startPos.x, targetY, startPos.z);
        currentTarget = startPos;
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
            if (atTop)
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

        transform.position = Vector3.MoveTowards(
            transform.position,
            currentTarget,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, currentTarget) < 0.0001f)
        {
            transform.position = currentTarget;
            moving = false;
            cooldownTimer = cooldown;

            atTop = (currentTarget == targetPos);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (moving || cooldownTimer > 0f) return;
        if (atTop) return;

        currentTarget = targetPos;
        moving = true;
    }
}
