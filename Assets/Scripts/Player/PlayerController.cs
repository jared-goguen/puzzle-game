using UnityEngine;

/// <summary>
/// Controls player movement with WASD input.
/// Movement happens on XZ plane (horizontal), respects collisions.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private CharacterController characterController;
    
    private Vector3 moveDirection = Vector3.zero;
    private float verticalVelocity = 0f;
    private const float GRAVITY = -9.81f;

    private void Start()
    {
        if (characterController == null)
            characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        HandleInput();
        HandleMovement();
    }

    /// <summary>
    /// Process WASD input.
    /// </summary>
    private void HandleInput()
    {
        float moveX = 0f;
        float moveZ = 0f;

        if (Input.GetKey(KeyCode.W))
            moveZ += 1f;
        if (Input.GetKey(KeyCode.S))
            moveZ -= 1f;
        if (Input.GetKey(KeyCode.D))
            moveX += 1f;
        if (Input.GetKey(KeyCode.A))
            moveX -= 1f;

        // Normalize diagonal movement
        Vector3 inputDirection = new Vector3(moveX, 0f, moveZ).normalized;

        // Rotate player to face movement direction
        if (inputDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(inputDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            moveDirection = inputDirection * moveSpeed;
        }
        else
        {
            moveDirection = Vector3.Lerp(moveDirection, Vector3.zero, Time.deltaTime * 5f);
        }
    }

    /// <summary>
    /// Apply movement with gravity (if using CharacterController).
    /// </summary>
    private void HandleMovement()
    {
        if (characterController != null && characterController.enabled)
        {
            // Add gravity
            verticalVelocity += GRAVITY * Time.deltaTime;
            Vector3 movement = moveDirection + Vector3.up * verticalVelocity;
            characterController.Move(movement * Time.deltaTime);

            // Reset vertical velocity when grounded
            if (characterController.isGrounded)
                verticalVelocity = 0f;
        }
        else
        {
            // Direct rigidbody or transform movement
            transform.Translate(moveDirection * Time.deltaTime, Space.World);
        }
    }

    /// <summary>
    /// Teleport player to a specific position (for room transitions).
    /// </summary>
    public void Teleport(Vector3 position)
    {
        if (characterController != null && characterController.enabled)
        {
            characterController.enabled = false;
            transform.position = position;
            characterController.enabled = true;
        }
        else
        {
            transform.position = position;
        }
    }

    /// <summary>
    /// Get current player position.
    /// </summary>
    public Vector3 GetPosition() => transform.position;
}
