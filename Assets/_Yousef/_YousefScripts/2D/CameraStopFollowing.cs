using UnityEngine;
using Unity.Cinemachine;

public class CameraStopFollowing : MonoBehaviour
{
    [SerializeField] float stopFollowingY = -10f;
    [SerializeField] CinemachineCamera virtualCamera;

    bool hasStopped;

    void Update()
    {
        if (hasStopped) return;

        if (transform.position.y <= stopFollowingY)
        {
            StopCameraFollow();
        }
    }

    void StopCameraFollow()
    {
        if (virtualCamera != null)
        {
            virtualCamera.Follow = null;
        }
        hasStopped = true;
    }
}