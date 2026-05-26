using UnityEngine;

/// <summary>
/// ScriptableObject definition for inventory items.
/// One asset per unique item type (key, book, note, etc).
/// </summary>
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class InventoryItem : ScriptableObject
{
    public string itemId;
    public string itemName = "Item";
    public string description = "An item";
    public Texture2D icon;
    public bool isStackable = false;
    public int maxStackSize = 1;

    /// <summary>
    /// Create a runtime copy of this item.
    /// </summary>
    public InventoryItemInstance CreateInstance()
    {
        return new InventoryItemInstance
        {
            itemId = this.itemId,
            itemName = this.itemName,
            description = this.description,
            quantity = 1
        };
    }
}

/// <summary>
/// Runtime instance of an inventory item (tracks quantity, etc).
/// </summary>
[System.Serializable]
public class InventoryItemInstance
{
    public string itemId;
    public string itemName;
    public string description;
    public int quantity = 1;
}
