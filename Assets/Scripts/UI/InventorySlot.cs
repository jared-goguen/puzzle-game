using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Individual inventory slot UI component.
/// Displays an item and handles drag-drop interactions.
/// </summary>
public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private Text itemNameText;
    [SerializeField] private Text quantityText;

    private InventoryItemInstance itemInstance;
    private CanvasGroup canvasGroup;
    private Vector3 startPosition;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    /// <summary>
    /// Set the item to display in this slot.
    /// </summary>
    public void SetItem(InventoryItemInstance item)
    {
        itemInstance = item;

        if (itemNameText != null)
            itemNameText.text = item.itemName;

        if (quantityText != null)
        {
            if (item.quantity > 1)
                quantityText.text = item.quantity.ToString();
            else
                quantityText.text = "";
        }

        // Icon would be set from InventoryItem ScriptableObject in a full implementation
    }

    /// <summary>
    /// Begin drag operation.
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
        startPosition = transform.position;
    }

    /// <summary>
    /// During drag operation.
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    /// <summary>
    /// End drag operation.
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        transform.position = startPosition;
    }

    /// <summary>
    /// Handle drop on this slot.
    /// </summary>
    public void OnDrop(PointerEventData eventData)
    {
        // Could implement item swapping/combining here
    }
}
