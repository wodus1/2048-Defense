using UnityEngine;

public class GoldParticleUI : MonoBehaviour // 골드 파티클 ui view
{
    [SerializeField] private RectTransform startRect;

    public void Initialize(Vector2 pos)
    {
        startRect.anchoredPosition = pos;
    }
}
