using UnityEngine;

public class YThirdPersonMovement : MonoBehaviour
{
    [SerializeField] Transform cameraObj;
    [SerializeField] float movementSpeed = 1f;
    [SerializeField] float playerRotationSpeed = 3f;

    Rigidbody rb;
    Vector2 playerInputs;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        ReadInputs();
    }

    void FixedUpdate()
    {
        WriteInputs();
    }

    void ReadInputs()
    {
        playerInputs.x = Input.GetAxisRaw("Horizontal");
        playerInputs.y = Input.GetAxisRaw("Vertical");
        playerInputs = Vector2.ClampMagnitude(playerInputs, 1f);
    }

    void WriteInputs()
    {
        Vector3 camForward = cameraObj.forward;
        Vector3 camRight = cameraObj.right;
        camForward.y = 0f;
        camRight.y = 0f;
        Vector3 move = camRight * playerInputs.x + camForward * playerInputs.y;

        Vector3 v = rb.linearVelocity;
        v = new Vector3(move.x * movementSpeed, v.y, move.z * movementSpeed);
        rb.linearVelocity = v;

        if (move.sqrMagnitude > 0.001f)
        {
            Quaternion rot = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, playerRotationSpeed * Time.fixedDeltaTime);
        }
    }
}
