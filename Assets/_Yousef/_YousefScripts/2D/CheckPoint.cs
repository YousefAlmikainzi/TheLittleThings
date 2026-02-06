using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] float respawnYOffset = 0.5f;

    bool activated;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (activated) return;

        var player = other.GetComponent<Y2DPlayerBehaviorWithHurt>();
        if (player == null) return;

        Vector3 checkpointPos = transform.position;
        checkpointPos.y += respawnYOffset;

        player.SetCheckpoint(checkpointPos);
        activated = true;
    }
}
