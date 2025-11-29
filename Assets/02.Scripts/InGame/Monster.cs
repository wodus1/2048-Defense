using System.Threading;
using UnityEngine;

public abstract class Monster : MonoBehaviour //몬스터 추상 클래스
{
    protected enum MonsterState
    {
        Move,
        Die
    }

    [SerializeField] protected MonsterAnimator monsterAnimator;
    protected abstract float NormalHp { get; }
    protected abstract float NormalSpeed { get; }

    public RectTransform rect;
    protected RectTransform canvasRect;
    protected float currentHp;

    protected void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        currentHp = NormalHp;
    }

    public virtual void TakeDamage(float damage)
    {
        if (currentHp <= 0f) return;

        currentHp -= damage;

        if (currentHp <= 0f)
        {
            OnDie();
        }
    }

    public Vector2 Position => new Vector2(transform.position.x, transform.position.z);

    public void MoveTo(Vector2 dir)
    {
        transform.position += (Vector3)(dir * NormalSpeed * Time.deltaTime);
    }

    public bool IsVisible()
    {
        Vector2 localPos = canvasRect.InverseTransformPoint(rect.position);

        float halfH = canvasRect.rect.height * 0.5f;

        return localPos.y <= halfH + 20f;
    }

    public abstract void OnDie();
}