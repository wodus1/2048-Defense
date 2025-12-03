using UnityEngine;

public class BlueMonster : Monster //탱 몬스터 오브젝트
{
    protected override float NormalHp => 50f;
    protected override float NormalSpeed => 12f;

    private void Start()
    {
        CurrentState = MonsterState.Move;
    }

    public override void Initialize(MonsterSystem monsterSystem, float hpMul)
    {
        this.monsterSystem = monsterSystem;
        currentHp = NormalHp * hpMul;
        CurrentState = MonsterState.Move;
    }

    private void Update()
    {
        switch (CurrentState)
        {
            case MonsterState.Move:
                monsterAnimator.PlayMove();
                MoveTo(Vector2.down);

                break;
            case MonsterState.Die:
                monsterAnimator.PlayDie();

                break;
        }
    }

    public override void OnDie()
    {
        CurrentState = MonsterState.Die;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Projectile>() is Projectile projectile)
        {
            TakeDamage(projectile.Damage);
        }
    }

    public void RetrunToPool()
    {
        monsterSystem.ReturnToPool(this);
    }
}