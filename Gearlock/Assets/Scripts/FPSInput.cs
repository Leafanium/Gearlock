using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FPSInput : MonoBehaviour
{
    public float speed = 6.0f;
    
    [Range(-20f, -1f)]
    public float gravity = -9.8f; 

    private CharacterController charController;
    private Vector3 movement;
    
    void Start()
    {
        charController = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Get movement input
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Get forward and right directions, ignoring vertical movement
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        forward.y = right.y = 0;

        forward.Normalize();
        right.Normalize();

        // Move in the correct direction
        Vector3 moveDirection = (forward * moveZ) + (right * moveX);
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

        movement = moveDirection * speed;
        movement.y += gravity * Time.deltaTime; // Apply gravity

        charController.Move(movement * Time.deltaTime);
    }
}
