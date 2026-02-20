using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagController : MonoBehaviour // 인벤토리 컨트롤러
{
    [SerializeField] private DragGhostController dragGhostController;
    [SerializeField] private List<InventorySlotUI> inventorySlots = new List<InventorySlotUI>();
    [SerializeField] private EquipSlotUI[] equipSlots;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(OnClickClose);

        for (int i = 0; i < equipSlots.Length; i++)
            equipSlots[i].Initialize(this, dragGhostController, i);
    }

    private void OnEnable()
    {
        int idx = 0;
        foreach(var item in SaveManager.Instance.PlayerData.items)
        {
            Sprite icon = GlobalLibraryManager.Instance.GetItemTemplate(item.Key).Item;
            inventorySlots[idx].Initialize(dragGhostController, item.Key, icon, item.Value);
            idx++;
        }

        RefreshEquipSlots();
    }

    private void RefreshEquipSlots()
    {
        for (int i = 0; i < equipSlots.Length; i++)
        {
            var title = SaveManager.Instance.PlayerData.equipped[i];
            
            if (string.IsNullOrEmpty(title))
            {
                equipSlots[i].Clear();
                continue;
            }

            var template = GlobalLibraryManager.Instance.GetItemTemplate(title);
            equipSlots[i].SetItem(template.Item);
        }
    }

    public void Equip(int index, string title)
    {
        SaveManager.Instance.PlayerData.EquipItem(title, index);
        RefreshEquipSlots();
    }

    public void Unequip(int index)
    {
        SaveManager.Instance.PlayerData.UnequipItem(index);
        RefreshEquipSlots();
    }

    private async void OnClickClose()
    {
        this.gameObject.SetActive(false);
        await SaveManager.Instance.SaveServerAsync();
    }
}