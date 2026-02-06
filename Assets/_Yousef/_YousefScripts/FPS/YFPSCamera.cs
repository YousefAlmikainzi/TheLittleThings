using UnityEngine;

public class YFPSCamera : MonoBehaviour
{
    [SerializeField] float sensitivity = 200.0f;
    [SerializeField] float lookUpLimit = -50.0f;
    [SerializeField] float lookDownLimit = 35.0f;

    float pitch = 0.0f;
    Vector2 cameraInputs;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        cameraMovment();
    }
    void cameraMovment()
    {
        cameraInputs.x = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up * cameraInputs.x, Space.World);
        cameraInputs.y = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        pitch -= cameraInputs.y;
        pitch = Mathf.Clamp(pitch, lookUpLimit, lookDownLimit);
        Vector3 currentAngle = transform.localEulerAngles;
        currentAngle.x = pitch;
        currentAngle.z = 0;
        transform.localEulerAngles = currentAngle;
    }
}
