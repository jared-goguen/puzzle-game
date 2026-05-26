using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Interactable object that can be picked up and added to inventory.
/// 
/// SETUP: Assign an InventoryItem ScriptableObject to the inventoryItemTemplate field.
/// </summary>
public class PickupItem : Interactable
{
    [SerializeField] private InventoryItem inventoryItemTemplate;
    [SerializeField] private bool destroyOnPickup = true;

    private void OnValidate()
    {
        // Editor-time validation: warn if item template is missing
        if (inventoryItemTemplate == null)
            Debug.LogWarning("[PickupItem] No inventory item template assigned!", this);
    }

    public override void OnInteract()
    {
        Assert.IsNotNull(inventoryItemTemplate, 
            $"[PickupItem] {gameObject.name} has no inventory item template!", this);

        // Add to inventory
        Inventory inventory = GameManager.Instance.GetInventory();
        Assert.IsNotNull(inventory, "[PickupItem] Inventory system not found!", this);

        // Check if item was successfully added to inventory
        if (!inventory.AddItem(inventoryItemTemplate))
        {
            Debug.LogWarning($"[PickupItem] Could not add {inventoryItemTemplate.itemName} - inventory full?");
            return;  // Don't destroy if not added
        }

        // Only mark and destroy if successfully added
        GameManager.Instance.GetGameState().PickupItem(interactableId);
        Debug.Log($"[PickupItem] Picked up: {inventoryItemTemplate.itemName}");

        // Remove from world
        if (destroyOnPickup)
            Destroy(gameObject);
    }
}
