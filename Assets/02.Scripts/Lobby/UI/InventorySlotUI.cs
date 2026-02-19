using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] Image iconImage;
    [SerializeField] TMP_Text countText;

    public void Initialize(Sprite icon, int count)
    {
        iconImage.sprite = icon;
        countText.SetText($"{count}");
    }
}
