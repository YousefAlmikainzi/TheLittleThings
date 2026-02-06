using UnityEngine;

public class LightFollowPlayer : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] float followSpeed = 10f;
    [SerializeField] float yOffset = 1f;

    Vector3 velocity;

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 targetPos = player.position + Vector3.up * yOffset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, 1f / followSpeed);
    }
}
