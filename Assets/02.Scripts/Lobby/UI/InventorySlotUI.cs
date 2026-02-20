using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler // 인벤토리 슬롯 ui(view + 드래그 입력 처리)
{
    private DragGhostController dragGhostController;
    
    [SerializeField] Image iconImage;
    [SerializeField] TMP_Text countText;

    public string ItemTitle { get; private set; }
    public Sprite ItemIcon => iconImage.sprite;
    public bool HasItem => !string.IsNullOrEmpty(ItemTitle) && iconImage.sprite != null;
    
    public void Initialize(DragGhostController dragGhostController, string itemTitle, Sprite icon, int count)
    {
        this.dragGhostController = dragGhostController;
        iconImage.sprite = icon;
        countText.SetText($"{count}");
        ItemTitle = itemTitle;
    }

    public void Clear()
    {
        ItemTitle = null;
        iconImage.sprite = null;
        countText.SetText($"{0}");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!HasItem) return;

        dragGhostController.Begin(ItemTitle, ItemIcon);
    }

    public void OnDrag(PointerEventData eventData)
    {
        dragGhostController.SetScreenPosition(eventData.position, eventData.pressEventCamera);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragGhostController.IsDragging)
            dragGhostController.End();
    }
}