using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemButtonUI : MonoBehaviour // 아이템 버튼 ui view
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Button button;
    [SerializeField] private Image blurImage;
    [SerializeField] private TMP_Text countText;

    private ItemSystem itemSystem;
    private ItemEffect effect;

    private bool isCooldown;

    public void Initialize(ItemSystem itemSystem, ItemEffect item = null, int count = 0)
    {
        this.itemSystem = itemSystem;
        button.onClick.RemoveAllListeners();
        countText.SetText($"{count}");

        if (count <= 0)
        {
            blurImage.fillAmount = 1;
            button.interactable = false;
        }
        else
        {
            blurImage.fillAmount = 0;
            button.interactable = true;
        }

        if (item == null) return;

        isCooldown = false;
        effect = item;
        iconImage.sprite = effect.Item;
        button.onClick.AddListener(OnClickButton);
    }

    private void OnClickButton()
    {
        if (isCooldown) return;

        itemSystem.Execute(effect);
        int cnt = SaveManager.Instance.PlayerData.GetItemCount(effect.Title);
        RefreshCount();

        if (effect is IDurationItem durationEffect)
        {
            StartCoroutine(Duration(durationEffect.Duration));
        }
    }

    private void RefreshCount()
    {
        int cnt = SaveManager.Instance.PlayerData.GetItemCount(effect.Title);
        countText.SetText($"{cnt}");

        if (cnt <= 0)
        {
            button.interactable = false;
            if (!isCooldown) blurImage.fillAmount = 1;
        }
        else
        {
            button.interactable = !isCooldown;
            if (!isCooldown) blurImage.fillAmount = 0;
        }
    }

    private IEnumerator Duration(float seconds)
    {
        isCooldown = true;
        button.interactable = false;

        float time = 0f;
        while (time < seconds)
        {
            time += Time.deltaTime;

            float ratio = Mathf.Clamp01(time / seconds);
            blurImage.fillAmount = 1f - ratio;

            yield return null;
        }

        isCooldown = false;

        int cnt = SaveManager.Instance.PlayerData.GetItemCount(effect.Title);
        button.interactable = cnt > 0;
        blurImage.fillAmount = (cnt > 0) ? 0 : 1;
    }
}
