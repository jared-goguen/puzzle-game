using UnityEngine;

/// <summary>
/// Base class for all interactive objects in the game.
/// Can be clicked to trigger actions (pickup, examine, use, etc).
/// 
/// SETUP: Assign a unique interactableId and displayName in the Inspector.
/// Renderer component is required for highlighting.
/// </summary>
public abstract class Interactable : MonoBehaviour
{
    [SerializeField] protected string interactableId;
    [SerializeField] protected string displayName = "Object";
    [SerializeField] protected string description = "An ordinary object.";
    [SerializeField] protected Color highlightColor = Color.yellow;

    protected Renderer objectRenderer;
    protected Material highlightMaterial;
    protected bool isHighlighted = false;

    protected virtual void OnValidate()
    {
        // Editor-time validation: warn if ID is missing
        if (string.IsNullOrEmpty(interactableId))
            Debug.LogWarning($"[Interactable] {gameObject.name} has no interactableId assigned", this);
    }

    protected virtual void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer == null)
            Debug.LogWarning($"[Interactable] {gameObject.name} has no Renderer component", this);
    }

    /// <summary>
    /// Called when player clicks this object.
    /// </summary>
    public abstract void OnInteract();

    /// <summary>
    /// Highlight this object (on hover).
    /// </summary>
    public virtual void Highlight()
    {
        if (isHighlighted || objectRenderer == null) return;

        isHighlighted = true;
        objectRenderer.material.color = highlightColor;
    }

    /// <summary>
    /// Remove highlight.
    /// </summary>
    public virtual void Unhighlight()
    {
        if (!isHighlighted || objectRenderer == null) return;

        isHighlighted = false;
        objectRenderer.material.color = Color.white;
    }

    /// <summary>
    /// Get object ID (for save/load tracking).
    /// </summary>
    public string GetInteractableId() => interactableId;

    /// <summary>
    /// Get display name.
    /// </summary>
    public string GetDisplayName() => displayName;

    /// <summary>
    /// Get description text.
    /// </summary>
    public string GetDescription() => description;
}
