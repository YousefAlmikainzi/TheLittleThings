using UnityEngine;

public class StompDetector : MonoBehaviour
{
    [SerializeField] float bounceForce = 8f;
    [SerializeField] float cooldown = 0.12f;

    Y2DPlayerBehaviorWithHurt player;
    Collider2D col;
    bool onCooldown;

    void Start()
    {
        player = GetComponentInParent<Y2DPlayerBehaviorWithHurt>();
        col = GetComponent<Collider2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (onCooldown) return;
        if (!other.CompareTag("Stompable")) return;

        if (player == null || player.GetComponent<Rigidbody2D>().linearVelocity.y >= 0f) return;

        player.Bounce(bounceForce);

        other.SendMessage("OnStomped", SendMessageOptions.DontRequireReceiver);

        StartCoroutine(Cooldown());
    }

    System.Collections.IEnumerator Cooldown()
    {
        onCooldown = true;
        if (col) col.enabled = false;
        yield return new WaitForSeconds(cooldown);
        if (col) col.enabled = true;
        onCooldown = false;
    }
}
