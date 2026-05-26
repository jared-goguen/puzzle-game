using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Isometric camera that follows the player.
/// Fixed angle: X=30-35°, Y=45° for that Blue Prince-style look.
/// 
/// SETUP: Assign the player's Transform to 'target' in the Inspector.
/// If not assigned, will attempt to find PlayerController in scene at runtime.
/// </summary>
public class IsometricCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float distance = 8f;
    [SerializeField] private float height = 6f;
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private float rotationX = 30f;
    [SerializeField] private float rotationY = 45f;
    
    // Orthographic settings for true isometric
    [SerializeField] private bool useOrthographic = true;
    [SerializeField] private float orthographicSize = 5f;

    private Camera cameraComponent;
    private Vector3 offset;

    private void OnValidate()
    {
        // Editor-time validation: warn if target is missing
        if (target == null)
        {
            Debug.LogWarning("[IsometricCamera] Target not assigned in Inspector. Will search scene at runtime.", this);
        }
    }

    private void Start()
    {
        cameraComponent = GetComponent<Camera>();
        
        // If not assigned in Inspector, find PlayerController in scene
        if (target == null)
        {
            PlayerController playerController = FindObjectOfType<PlayerController>();
            Assert.IsNotNull(playerController, "PlayerController not found in scene", this);
            target = playerController.transform;
        }

        if (useOrthographic)
            cameraComponent.orthographic = true;
        else
            cameraComponent.orthographic = false;

        CalculateOffset();
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Calculate desired position
        Vector3 desiredPosition = target.position + offset;
        
        // Smoothly move camera
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Maintain isometric angle (look at target)
        transform.LookAt(target.position + Vector3.up * (height * 0.5f));
    }

    /// <summary>
    /// Calculate offset based on rotation angles.
    /// </summary>
    private void CalculateOffset()
    {
        Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0f);
        offset = rotation * (Vector3.back * distance + Vector3.up * height);
    }

    /// <summary>
    /// Set orthographic size (zoom level).
    /// </summary>
    public void SetZoom(float size)
    {
        if (useOrthographic)
            cameraComponent.orthographicSize = size;
    }

    /// <summary>
    /// Toggle between orthographic and perspective.
    /// </summary>
    public void SetOrthographic(bool orthographic)
    {
        useOrthographic = orthographic;
        cameraComponent.orthographic = orthographic;
    }
}
