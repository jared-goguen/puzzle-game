using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI for displaying the inventory grid.
/// Shows items visually, supports drag-drop interaction.
/// </summary>
public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Transform inventoryGrid;
    [SerializeField] private GameObject inventorySlotPrefab;
    [SerializeField] private Inventory inventory;
    [SerializeField] private Toggle inventoryToggle;

    private bool inventoryOpen = false;

    private void Start()
    {
        if (inventory == null)
            inventory = GetComponent<Inventory>();

        // Subscribe to inventory changes
        if (inventory != null)
            inventory.OnInventoryChanged += RefreshDisplay;
        else
            Debug.LogError("[InventoryUI] Inventory component not found!");

        if (inventoryToggle != null)
            inventoryToggle.onValueChanged.AddListener(OnInventoryToggle);

        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        // Unsubscribe from inventory changes to avoid memory leaks
        if (inventory != null)
            inventory.OnInventoryChanged -= RefreshDisplay;
    }

    private void Update()
    {
        // Toggle inventory with I key
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    /// <summary>
    /// Toggle inventory panel visibility.
    /// </summary>
    public void ToggleInventory()
    {
        inventoryOpen = !inventoryOpen;
        if (inventoryPanel != null)
            inventoryPanel.SetActive(inventoryOpen);
    }

    /// <summary>
    /// Refresh the inventory display.
    /// </summary>
    public void RefreshDisplay()
    {
        if (inventoryGrid == null)
            return;

        // Clear old slots
        foreach (Transform child in inventoryGrid)
        {
            Destroy(child.gameObject);
        }

        // Create new slots
        var items = inventory.GetAllItems();
        foreach (var item in items)
        {
            GameObject slotGO = Instantiate(inventorySlotPrefab, inventoryGrid);
            InventorySlot slot = slotGO.GetComponent<InventorySlot>();
            if (slot != null)
                slot.SetItem(item);
        }
    }

    /// <summary>
    /// Handle inventory toggle change.
    /// </summary>
    private void OnInventoryToggle(bool isOn)
    {
        if (inventoryPanel != null)
            inventoryPanel.SetActive(isOn);
    }
}
