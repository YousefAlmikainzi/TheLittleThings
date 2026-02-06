using UnityEngine;

public class Y3DM: MonoBehaviour
{
    [SerializeField] float walkSpeed = 10.0f;
    [SerializeField] Transform cameraMovement;

    Rigidbody rb;
    Vector2 playerInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        readInput();
    }
    void FixedUpdate()
    {
        writeInputs();
    }
    void readInput()
    {
        playerInput.x = Input.GetAxisRaw("Horizontal");
        playerInput.y = Input.GetAxisRaw("Vertical");
        playerInput = Vector2.ClampMagnitude(playerInput, 1);
    }
    void writeInputs()
    {
        Vector3 cameraForward = cameraMovement.forward;
        Vector3 cameraRight = cameraMovement.right;
        cameraForward.y = 0;
        cameraForward.Normalize();
        cameraRight.y = 0;
        cameraRight.Normalize();

        Vector3 movementTotal = cameraForward * playerInput.y + cameraRight * playerInput.x;
        movementTotal.Normalize();

        Vector3 v = rb.linearVelocity;
        v.x = movementTotal.x * walkSpeed;
        v.z = movementTotal.z * walkSpeed;
        rb.linearVelocity = v;
    }
}
/*using UnityEngine;

public class Y3DMovement : MonoBehaviour
{
    [SerializeField] float walkSpeed = 10.0f;
    [SerializeField] Transform cameraMovement;

    Rigidbody rb;
    Animator animator;

    Vector2 playerInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        readInput();
        updateAnimation();
    }

    void FixedUpdate()
    {
        writeInputs();
    }

    void readInput()
    {
        playerInput.x = Input.GetAxisRaw("Horizontal");
        playerInput.y = Input.GetAxisRaw("Vertical");
        playerInput = Vector2.ClampMagnitude(playerInput, 1);
    }

    void writeInputs()
    {
        Vector3 cameraForward = cameraMovement.forward;
        Vector3 cameraRight = cameraMovement.right;

        cameraForward.y = 0;
        cameraRight.y = 0;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 movementTotal = cameraForward * playerInput.y + cameraRight * playerInput.x;
        movementTotal.Normalize();

        Vector3 v = rb.linearVelocity;
        v.x = movementTotal.x * walkSpeed;
        v.z = movementTotal.z * walkSpeed;
        rb.linearVelocity = v;
    }

    void updateAnimation()
    {
        bool isPressingW = Input.GetKey(KeyCode.W);
        animator.SetBool("walking", isPressingW);
    
    }
}*/

