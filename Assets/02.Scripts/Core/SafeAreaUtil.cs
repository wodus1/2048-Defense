using UnityEngine;

public class SafeAreaUtil : MonoBehaviour
{
    public static Rect GetSafeAreaInCanvas(RectTransform canvasRect)
    {
        Rect safe = Screen.safeArea;

        Vector2 anchorMin = safe.position;
        Vector2 anchorMax = safe.position + safe.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        Vector2 size = canvasRect.rect.size;
        Vector2 pivot = canvasRect.pivot;

        Vector2 localMin = new Vector2(
            (anchorMin.x - pivot.x) * size.x,
            (anchorMin.y - pivot.y) * size.y
        );
        Vector2 localMax = new Vector2(
            (anchorMax.x - pivot.x) * size.x,
            (anchorMax.y - pivot.y) * size.y
        );

        return Rect.MinMaxRect(localMin.x, localMin.y, localMax.x, localMax.y);
    }
}
