using UnityEngine;

public class BackNForth : MonoBehaviour
{
    [SerializeField] private float amplitude = 5f;
    [SerializeField] private float speed = 2f;
    [SerializeField] private bool startMovingRight = true;

    private Vector3 startPosition;
    private SpriteRenderer spriteRenderer;
    private float direction;

    void Start()
    {
        startPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        direction = startMovingRight ? 1f : -1f;

        if (spriteRenderer != null)
            spriteRenderer.flipX = startMovingRight;
    }

    void Update()
    {
        transform.position += Vector3.right * direction * speed * Time.deltaTime;

        float offset = transform.position.x - startPosition.x;

        if (offset > amplitude)
        {
            transform.position = new Vector3(startPosition.x + amplitude, transform.position.y, transform.position.z);
            Flip();
        }
        else if (offset < -amplitude)
        {
            transform.position = new Vector3(startPosition.x - amplitude, transform.position.y, transform.position.z);
            Flip();
        }
    }

    void Flip()
    {
        direction *= -1f;

        if (spriteRenderer != null)
            spriteRenderer.flipX = !spriteRenderer.flipX;
    }
}
