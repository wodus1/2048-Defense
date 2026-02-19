using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour // 상점 아이템 ui view
{
    StoreController storeController;

    [SerializeField] Button button;
    [SerializeField] Image itemImage;
    [SerializeField] Image blurImage;
    [SerializeField] TMP_Text titleText;
    [SerializeField] TMP_Text descriptionText;
    [SerializeField] TMP_Text priceText;

    private int price;

    public string Title => titleText.text;
    public int Price => price;

    public void Initialize(StoreController storeController, Sprite item, string title, string description, string price)
    {
        this.storeController = storeController;
        itemImage.sprite = item;
        titleText.text = title;
        descriptionText.text = description;
        priceText.text = price;
        this.price = int.Parse(price);

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClickButton);
    }

    public void Refesh(int gold)
    {
        if (gold < price)
        {
            button.interactable = false;
            blurImage.gameObject.SetActive(true);
        }
        else
        {
            button.interactable = true;
            blurImage.gameObject.SetActive(false);
        }
    }

    private void OnClickButton()
    {
        storeController.OnClickShopItem(this);
    }
}
