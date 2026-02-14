using UnityEngine;

public class BugMonster : Monster // 공중 몬스터 오브젝트
{
    protected override float NormalHp => 30f;
    protected override float NormalSpeed => 19f;
    protected override int NormalAttackDamage => 7;

    private HpSystem hpSystem;
    private float attackTime = 0;

    private void Start()
    {
        CurrentState = MonsterState.Move;
    }

    public override void Initialize(MonsterSystem monsterSystem, HpSystem hpSystem, float hpMul)
    {
        this.monsterSystem = monsterSystem;
        this.hpSystem = hpSystem;

        currentHp = NormalHp * hpMul;
        CurrentState = MonsterState.Move;
        attackTime = 0;
    }

    private void Update()
    {
        if (monsterSystem.IsPause())
            return;

        switch (CurrentState)
        {
            case MonsterState.Move:
                monsterAnimator.PlayMove();
                MoveTo(Vector2.down);

                break;
            case MonsterState.Die:
                monsterAnimator.PlayDie();

                break;
            case MonsterState.Attack:
                attackTime += Time.deltaTime;

                if (attackTime >= 1f)
                {
                    attackTime = 0;
                    hpSystem.TakeDamage(NormalAttackDamage);
                }
                break;
        }
    }

    public override void OnDie()
    {
        CurrentState = MonsterState.Die;
        attackTime = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Projectile>() is Projectile projectile)
        {
            TakeDamage(projectile.Damage);
        }

        if (collision.CompareTag("HpBar"))
        {
            CurrentState = MonsterState.Attack;
        }
    }

    public void RetrunToPool()
    {
        monsterSystem.ReturnToPool(this);
    }
}