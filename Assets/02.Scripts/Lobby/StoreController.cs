using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreController : MonoBehaviour // 상점 컨트롤러
{
    private List<ShopItemUI> shopItems = new List<ShopItemUI>();
    [SerializeField] GoldUI goldUI;
    [SerializeField] ShopItemUI shopItemPrefab;
    [SerializeField] RectTransform content;
    [SerializeField] Button closeButton;

    void Start()
    {
        foreach(ItemEffect item in GlobalLibraryManager.Instance.ItemLibrary.templates)
        {
            var shopItem = Instantiate(shopItemPrefab, content);
            shopItem.Initialize(this, item.Item, item.Title, item.Description, item.Price);
            shopItem.Refesh(SaveManager.Instance.PlayerData.gold);
            shopItems.Add(shopItem);
        }

        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(OnClickClose);
    }

    private async void OnClickClose()
    {
        this.gameObject.SetActive(false);
        await SaveManager.Instance.SaveServerAsync();
    }

    public void OnClickShopItem(ShopItemUI item)
    {
        if(SaveManager.Instance.PlayerData.TryConsumeGold(item.Price))
        {
            SaveManager.Instance.PlayerData.AddItem(item.Title, 1);

            GoldRefresh();
            foreach (ShopItemUI shopItem in shopItems)
            {
                shopItem.Refesh(SaveManager.Instance.PlayerData.gold);
            }
        }
    }

    public void GoldRefresh()
    {
        goldUI.Refresh(SaveManager.Instance.PlayerData.gold);
    }
}
