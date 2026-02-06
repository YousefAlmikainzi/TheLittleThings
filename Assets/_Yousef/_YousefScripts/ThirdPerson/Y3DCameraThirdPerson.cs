using UnityEngine;

public class Y3DCameraThirdPerson : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Vector3 cameraOffset = new Vector3(0, 3, -6);
    [SerializeField] float cameraSensitivity = 3.0f;
    [SerializeField] float lookUpOffset = 1.0f;

    float yaw;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        yaw += Input.GetAxis("Mouse X") * cameraSensitivity;

        Quaternion rotation = Quaternion.Euler(0, yaw, 0);
        transform.position = player.position + rotation * cameraOffset;

        Vector3 lookTarget = player.position + Vector3.up * lookUpOffset;
        transform.LookAt(lookTarget);
    }
}
