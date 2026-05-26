using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Player inventory system.
/// Manages items, allows adding/removing, checking inventory state.
/// Uses events to decouple from UI - InventoryUI subscribes to OnInventoryChanged.
/// 
/// SETUP: InventoryUI component should be on the same GameObject to receive events.
/// </summary>
public class Inventory : MonoBehaviour
{
    [SerializeField] private int maxSlots = 20;
    
    private List<InventoryItemInstance> items = new List<InventoryItemInstance>();

    /// <summary>
    /// Event fired when inventory contents change (item added/removed).
    /// Allows UI to update without direct coupling.
    /// </summary>
    public delegate void InventoryChangedDelegate();
    public event InventoryChangedDelegate OnInventoryChanged;

    private void Start()
    {
        // InventoryUI subscribes to OnInventoryChanged in its Start()
    }

    /// <summary>
    /// Add item to inventory.
    /// </summary>
    public bool AddItem(InventoryItem itemTemplate)
    {
        if (items.Count >= maxSlots)
        {
            Debug.LogWarning("[Inventory] Inventory is full!");
            return false;
        }

        if (itemTemplate.isStackable)
        {
            // Check if item already exists
            InventoryItemInstance existingItem = items.Find(i => i.itemId == itemTemplate.itemId);
            if (existingItem != null && existingItem.quantity < itemTemplate.maxStackSize)
            {
                existingItem.quantity++;
                OnInventoryChanged?.Invoke();  // Notify listeners of change
                return true;
            }
        }

        // Add new instance
        InventoryItemInstance newItem = itemTemplate.CreateInstance();
        items.Add(newItem);
        Debug.Log($"[Inventory] Added: {itemTemplate.itemName}");

        OnInventoryChanged?.Invoke();  // Notify listeners of change

        return true;
    }

    /// <summary>
    /// Remove item from inventory.
    /// </summary>
    public bool RemoveItem(string itemId, int quantity = 1)
    {
        InventoryItemInstance item = items.Find(i => i.itemId == itemId);
        if (item == null)
            return false;

        item.quantity -= quantity;
        if (item.quantity <= 0)
            items.Remove(item);

        OnInventoryChanged?.Invoke();  // Notify listeners of change

        return true;
    }

    /// <summary>
    /// Check if inventory contains an item.
    /// </summary>
    public bool HasItem(string itemId)
    {
        return items.Exists(i => i.itemId == itemId);
    }

    /// <summary>
    /// Get all items in inventory.
    /// </summary>
    public List<InventoryItemInstance> GetAllItems() => new List<InventoryItemInstance>(items);

    /// <summary>
    /// Load items (for save/load).
    /// </summary>
    public void LoadItems(List<InventoryItemInstance> loadedItems)
    {
        items = loadedItems != null ? new List<InventoryItemInstance>(loadedItems) : new List<InventoryItemInstance>();
        OnInventoryChanged?.Invoke();  // Notify listeners of change
    }

    /// <summary>
    /// Clear inventory (for new game).
    /// </summary>
    public void Clear()
    {
        items.Clear();
        OnInventoryChanged?.Invoke();  // Notify listeners of change
    }

    /// <summary>
    /// Get inventory slot count.
    /// </summary>
    public int GetSlotCount() => items.Count;

    /// <summary>
    /// Get max slots.
    /// </summary>
    public int GetMaxSlots() => maxSlots;
}
