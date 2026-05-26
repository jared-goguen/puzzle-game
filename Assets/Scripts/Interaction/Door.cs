using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

/// <summary>
/// Door interactable that transitions between rooms.
/// 
/// SETUP: Assign a target scene name in the Inspector.
/// Optionally assign a spawn position or place a spawn point in the target room.
/// </summary>
public class Door : Interactable
{
    [SerializeField] private string targetSceneName;
    [SerializeField] private Vector3 spawnPosition = Vector3.zero;
    [SerializeField] private bool requiresItem = false;
    [SerializeField] private InventoryItem requiredItem;

    private void OnValidate()
    {
        // Editor-time validation: warn if target scene is missing
        if (string.IsNullOrEmpty(targetSceneName))
            Debug.LogWarning("[Door] Target scene name not assigned in Inspector!", this);
    }

    public override void OnInteract()
    {
        // Check if item required
        if (requiresItem && requiredItem != null)
        {
            Inventory inventory = GameManager.Instance.GetInventory();
            if (!inventory.HasItem(requiredItem.itemId))
            {
                Debug.Log($"[Door] You need {requiredItem.itemName} to enter this room.");
                DescriptionPanel descPanel = FindObjectOfType<DescriptionPanel>();
                if (descPanel != null)
                    descPanel.ShowDescription("Locked", $"You need {requiredItem.itemName} to enter.");
                return;
            }
        }

        // Save player position before transitioning
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            GameManager.Instance.GetGameState().SetPlayerPosition(playerController.GetPosition());
        }
        else
        {
            Debug.LogWarning("[Door] PlayerController not found when saving position!");
        }

        // Load target scene
        Assert.IsFalse(string.IsNullOrEmpty(targetSceneName), 
            $"[Door] No target scene specified for door {gameObject.name}", this);
        
        SceneManager.LoadScene(targetSceneName);
    }

    /// <summary>
    /// Get the spawn position for the player in the target room.
    /// </summary>
    public Vector3 GetSpawnPosition() => spawnPosition;
}
