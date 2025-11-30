using System.Threading;
using UnityEngine;

public abstract class Monster : MonoBehaviour //몬스터 추상 클래스
{
    public enum MonsterState
    {
        Move,
        Die
    }

    [SerializeField] protected MonsterAnimator monsterAnimator;
    protected abstract float NormalHp { get; }
    protected abstract float NormalSpeed { get; }

    public RectTransform Rect;
    protected RectTransform canvasRect;
    protected float currentHp;
    protected MonsterSystem monsterSystem;
    public MonsterState CurrentState;

    protected void Awake()
    {
        Rect = GetComponent<RectTransform>();
        canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        currentHp = NormalHp;
    }

    public abstract void Initialize(MonsterSystem monsterSystem, float hpMul);

    public virtual void TakeDamage(float damage)
    {
        if (currentHp <= 0f) return;

        currentHp -= damage;

        if (currentHp <= 0f)
        {
            OnDie();
        }
    }

    public void MoveTo(Vector2 dir)
    {
        transform.position += (Vector3)(dir * NormalSpeed * Time.deltaTime);
    }

    public bool IsVisible()
    {
        Vector2 localPos = canvasRect.InverseTransformPoint(Rect.position);

        float halfH = canvasRect.rect.height * 0.5f;

        return localPos.y <= halfH + 20f;
    }

    public abstract void OnDie();
}