using UnityEngine;

public class BlueMonster : Monster //몬스터 오브젝트
{
    protected override float NormalHp => 50f;
    protected override float NormalSpeed => 12f;

    private MonsterSystem monsterSystem;
    private MonsterState monsterState;

    private void Start()
    {
        monsterState = MonsterState.Move;
    }

    public void Initialize(MonsterSystem monsterSystem)
    {
        this.monsterSystem = monsterSystem;
    }

    private void Update()
    {
        switch (monsterState)
        {
            case MonsterState.Move:
                monsterAnimator.PlayMove();
                MoveTo(Vector2.down);

                break;
            case MonsterState.Die:
                monsterAnimator.PlayDie();
                monsterSystem.ReturnToPool(this);

                break;
        }
    }

    public override void OnDie()
    {
        monsterState = MonsterState.Die;
    }
}