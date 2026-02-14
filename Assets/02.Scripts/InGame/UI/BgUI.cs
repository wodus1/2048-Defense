using UnityEngine;

public class BgUI : MonoBehaviour // 배경 ui 초기화
{
    [SerializeField] RectTransform canvasRect;
    [SerializeField] RectTransform bgTopImage;
    [SerializeField] RectTransform bgBottomImage;

    public void Start()
    {
        float height = canvasRect.rect.height * 0.5f;

        bgTopImage.offsetMin = new Vector2(0f, height);
        bgTopImage.offsetMax = new Vector2(0f, 0f);

        bgBottomImage.offsetMin = new Vector2(0f, 0f);
        bgBottomImage.offsetMax = new Vector2(0f, -height);
    }
}