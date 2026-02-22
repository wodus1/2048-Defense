using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public abstract class Monster : MonoBehaviour // 몬스터 추상 클래스
{
    public enum MonsterState
    {
        Move,
        Die,
        Attack
    }

    [SerializeField] protected MonsterAnimator monsterAnimator;
    protected abstract float NormalHp { get; }
    protected abstract float NormalSpeed { get; }
    protected abstract int NormalAttackDamage { get; }

    public RectTransform Rect;
    protected RectTransform canvasRect;
    protected float currentHp;
    protected MonsterSystem monsterSystem;
    public MonsterState CurrentState;
    private Rect safeAreaRect;
    private Image image;
    private Color damageColor = new Color32(255, 171, 171, 255);
    private Color normalColor = Color.white;
    private Tween hitTween;

    protected void Awake()
    {
        Rect = GetComponent<RectTransform>();
        canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        image = GetComponent<Image>();
        safeAreaRect = SafeAreaUtil.GetSafeAreaInCanvas(canvasRect);
        currentHp = NormalHp;
    }

    public abstract void Initialize(MonsterSystem monsterSystem, HpSystem hpSystem, float hpMul);

    public virtual void TakeDamage(float damage)
    {
        if (currentHp <= 0f) return;

        hitTween?.Kill(false);
        hitTween = image.DOColor(damageColor, 0.06f)
                     .SetEase(Ease.OutQuad)
                     .OnComplete(() =>
                     {
                         hitTween = image.DOColor(normalColor, 0.12f)
                                         .SetEase(Ease.OutQuad);
                     });

        currentHp -= damage;

        if (currentHp <= 0f)
        {
            OnDie();
        }
    }

    public void MoveTo(Vector2 dir)
    {
        Rect.anchoredPosition += dir * NormalSpeed * Time.deltaTime;
    }

    public bool IsVisible()
    {
        Vector2 localPos = canvasRect.InverseTransformPoint(Rect.position);
        float topSafeY = safeAreaRect.yMax;

        return localPos.y <= topSafeY + 10f;
    }

    public abstract void OnDie();
}