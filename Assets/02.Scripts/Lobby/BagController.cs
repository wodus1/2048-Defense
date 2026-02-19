using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class BagController : MonoBehaviour
{
    [SerializeField] private List<InventorySlotUI> inventorySlots = new List<InventorySlotUI>();
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(OnClickClose);
    }

    private void OnEnable()
    {
        int idx = 0;
        foreach(var item in SaveManager.Instance.PlayerData.items)
        {
            Sprite icon = GlobalLibraryManager.Instance.GetItemTemplate(item.Key).Item;
            inventorySlots[idx].Initialize(icon, item.Value);
            idx++;
        }
    }

    private async void OnClickClose()
    {
        this.gameObject.SetActive(false);
    }
}
