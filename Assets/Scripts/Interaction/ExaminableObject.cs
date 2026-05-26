using UnityEngine;

/// <summary>
/// Interactable object that shows a description when clicked.
/// Doesn't get picked up, but provides story/environment information.
/// </summary>
public class ExaminableObject : Interactable
{
    public override void OnInteract()
    {
        // Display description in UI
        DescriptionPanel descPanel = FindObjectOfType<DescriptionPanel>();
        if (descPanel != null)
        {
            descPanel.ShowDescription(displayName, description);
        }
        else
        {
            Debug.Log($"[ExaminableObject] {displayName}: {description}");
        }
    }
}
