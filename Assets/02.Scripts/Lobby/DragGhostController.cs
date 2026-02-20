using UnityEngine;
using UnityEngine.UI;

public class DragGhostController : MonoBehaviour // 인벤토리 더미 아이템 ui 컨트롤러
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Image itemGhostIcon;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform rectTransform;

    private Vector2 offset = new(10, 10);
    private float alpha = 0.5f;

    public bool IsDragging { get; private set; }
    public string ItemTitle { get; private set; }
    public Sprite ItemIcon { get; private set; }

    private void Awake()
    {
        Hide();
    }

    public void SetScreenPosition(Vector2 screenPos, Camera uiCamera)
    {
        if (!IsDragging) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)canvas.transform,
            screenPos,
            uiCamera,
            out var localPoint
        );

        rectTransform.anchoredPosition = localPoint + offset;
    }

    public void Begin(string tile, Sprite icon)
    {
        IsDragging = true;
        ItemTitle = tile;
        ItemIcon = icon;
        itemGhostIcon.sprite = icon;

        canvasGroup.alpha = alpha;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        gameObject.SetActive(true);
    }

    public void End()
    {
        IsDragging = false;
        ItemTitle = null;
        ItemIcon = null;
        Hide();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
