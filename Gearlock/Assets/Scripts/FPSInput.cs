using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FPSInput : MonoBehaviour
{
    public float speed = 6.0f;
    [Range(-20f, -1f)]
    public float gravity = -9.8f;
    public float jumpHeight = 3.0f; // Jump strength

    private CharacterController charController;
    private Vector3 movement;
    private bool isGrounded;

    void Start()
    {
        charController = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Check if on the ground
        isGrounded = charController.isGrounded;

        // Get movement input
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        forward.y = right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = (forward * moveZ) + (right * moveX);
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

        movement.x = moveDirection.x * speed;
        movement.z = moveDirection.z * speed;

        // Jumping
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            movement.y = Mathf.Sqrt(jumpHeight * -2f * gravity); // Physics-based jump calculation
        }

        // Apply gravity
        if (!isGrounded)
        {
            movement.y += gravity * Time.deltaTime;
        }

        charController.Move(movement * Time.deltaTime);
    }
}
