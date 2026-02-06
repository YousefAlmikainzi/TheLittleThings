using UnityEngine;

public class RunCheckPoint : MonoBehaviour
{
    [SerializeField] float respawnYOffset = 0.5f;
    [SerializeField] float speedBonus = 2f;
    [SerializeField] float jumpBonus = 2f;
    [SerializeField] float gravityBonus = -0.5f;

    bool activated;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (activated) return;

        var player = other.GetComponent<Y2DPlayerBehaviorWithHurt>();
        if (player == null) return;

        Vector3 checkpointPos = transform.position;
        checkpointPos.y += respawnYOffset;
        player.SetCheckpoint(checkpointPos);

        var movement = player.GetComponent<Y2DMovementFinal>();
        if (movement != null)
        {
            movement.AddSpeed(speedBonus);
            movement.AddJumpForce(jumpBonus);
            movement.AddGravityScale(gravityBonus);
        }

        activated = true;
    }
}
