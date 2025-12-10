using DG.Tweening;
using UnityEngine;

public class HpUI : MonoBehaviour //Ã¼·Â ui view
{
    [SerializeField] private RectTransform hpBar;
    [SerializeField] private RectTransform hpBarFill;
    private float hpWidth;

    private void Awake()
    {
        hpWidth = hpBarFill.sizeDelta.x;
    }

    public void SetHP(float current)
    {
        float ratio = Mathf.Clamp01(current * 0.01f);

        var size = hpBarFill.sizeDelta;
        size.x = hpWidth * ratio;
        hpBarFill.sizeDelta = size;
    }

    public void DamageAnimation()
    {
        hpBar.DOKill();
        hpBar.DOShakeAnchorPos(
            duration: 0.2f,
            strength: new Vector2(3f, 3f),
            vibrato: 20,
            randomness: 90,
            snapping: false,
            fadeOut: true
        );
    }
}