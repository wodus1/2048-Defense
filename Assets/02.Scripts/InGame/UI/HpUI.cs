using UnityEngine;

public class HpUI : MonoBehaviour
{
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
}