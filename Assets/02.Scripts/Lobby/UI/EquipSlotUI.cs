using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipSlotUI : MonoBehaviour, IDropHandler // 인벤토리 장착 슬롯 ui view

{
    private BagController bagController;
    private DragGhostController dragGhostController;

    [SerializeField] private Image iconImage;
    [SerializeField] private Button unequipButton;

    private int index;

    public void Initialize(BagController bagController, DragGhostController dragGhostController, int index)
    {
        this.bagController = bagController;
        this.dragGhostController = dragGhostController;
        this.index = index;

        unequipButton.onClick.RemoveAllListeners();
        unequipButton.onClick.AddListener(OnClickUnequip);
    }

    public void SetItem(Sprite icon)
    {
        unequipButton.interactable = true;
        iconImage.sprite = icon;
    }

    public void Clear()
    {
        iconImage.sprite = null;
        unequipButton.interactable = false;
    }

    private void OnClickUnequip()
    {
        bagController.Unequip(index);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!dragGhostController.IsDragging) return;

        string title = dragGhostController.ItemTitle;
        if (string.IsNullOrEmpty(title)) return;

        bagController.Equip(index, title);
        dragGhostController.End();
    }
}
