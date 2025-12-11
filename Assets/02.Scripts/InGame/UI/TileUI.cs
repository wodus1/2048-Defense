using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TileUI : MonoBehaviour //2048타일 ui view
{
    [SerializeField] private Image bgImage;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private RectTransform rectTransform;
    private Dictionary<int, Color32> colors = new Dictionary<int, Color32>()
    {
        { 0,     new Color32(255, 255, 255, 255) },
        { 2,     new Color32(237, 227, 217, 255) },
        { 4,     new Color32(237, 224, 199, 255) },
        { 8,     new Color32(242, 176, 120, 255) },
        { 16,    new Color32(245, 148, 99, 255) },
        { 32,    new Color32(245, 125, 94, 255) },
        { 64,    new Color32(245, 94, 58, 255) },
        { 128,   new Color32(237, 207, 115, 255) },
        { 256,   new Color32(237, 204, 97, 255) },
        { 512,   new Color32(237, 199, 79, 255) },
        { 1024,  new Color32(237, 196, 63, 255) },
        { 2048,  new Color32(237, 194, 45, 255) },
        { 4096,  new Color32(77, 169, 255, 255) },
        { 8192,  new Color32(59, 107, 255, 255) },
        { 16384, new Color32(110, 45, 255, 255) },
        
        //16384 보다 큰 모든 값
        { -1, new Color32(51, 128, 186, 255) }
    };
    private int value;

    public RectTransform RectTransform;

    public void SetValue(int value)
    {
        int oldValue = this.value;
        this.value = value;

        if (this.value == 0)
            valueText.text = "";
        else
            valueText.text = this.value.ToString();

        UpdateColor(this.value);

        if (this.value != 0 && this.value != oldValue)
        {
            PlayPopAnimation();
        }
    }

    void UpdateColor(int value)
    {
        Color32 col;
        if (!colors.TryGetValue(value, out col))
        {
            col = colors[-1];
        }
        bgImage.color = col;
    }

    public void PlayPopAnimation()
    {
        rectTransform.DOKill();
        Sequence seq = DOTween.Sequence();
        seq.Append(rectTransform.DOScale(1.1f, 0.07f).SetEase(Ease.OutBack));
        seq.Append(rectTransform.DOScale(1.0f, 0.07f).SetEase(Ease.Linear));
    }

    public void PlaySpawn()
    {
        RectTransform.localScale = Vector3.zero;
        RectTransform.DOScale(1f, 0.1f).SetEase(Ease.OutBack);
    }

    public Tween PlayMove(Vector2 endPos, float duration, bool isSecondMerged, int newValue)
    {
        Tween tween = RectTransform.DOAnchorPos(endPos, duration)
            .SetEase(Ease.OutQuad);

        if (isSecondMerged)
        {
            tween.OnComplete(() =>
            {
                gameObject.SetActive(false);
                SetValue(0);
            });
        }
        else
        {
            tween.OnComplete(() =>
            {
                SetValue(newValue);
            });
        }

        return tween;
    }
}