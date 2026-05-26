using UnityEngine;

/// <summary>
/// Manages cursor visuals and raycasting for interaction detection.
/// Highlights interactable objects on hover.
/// </summary>
public class CursorManager : MonoBehaviour
{
    [SerializeField] private float raycastDistance = 1000f;
    [SerializeField] private LayerMask interactableLayer = -1;
    [SerializeField] private Texture2D normalCursor;
    [SerializeField] private Texture2D interactCursor;

    private Interactable currentHoveredInteractable;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        Cursor.SetCursor(normalCursor, Vector2.zero, CursorMode.Auto);
    }

    private void Update()
    {
        HandleInteractionRaycast();
        HandleInteractionClick();
    }

    /// <summary>
    /// Cast ray from camera to detect interactables.
    /// </summary>
    private void HandleInteractionRaycast()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        // Clear previous hover
        if (currentHoveredInteractable != null)
            currentHoveredInteractable.Unhighlight();

        // Raycast
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, raycastDistance, interactableLayer))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            if (interactable != null)
            {
                currentHoveredInteractable = interactable;
                interactable.Highlight();

                // Change cursor
                if (interactCursor != null)
                    Cursor.SetCursor(interactCursor, Vector2.zero, CursorMode.Auto);
                else
                    Cursor.SetCursor(normalCursor, Vector2.zero, CursorMode.Auto);

                return;
            }
        }

        // No interactable hit
        currentHoveredInteractable = null;
        if (normalCursor != null)
            Cursor.SetCursor(normalCursor, Vector2.zero, CursorMode.Auto);
    }

    /// <summary>
    /// Handle click on interactable.
    /// </summary>
    private void HandleInteractionClick()
    {
        if (Input.GetMouseButtonDown(0) && currentHoveredInteractable != null)
        {
            currentHoveredInteractable.OnInteract();
        }
    }
}
