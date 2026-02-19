using UnityEngine;

public abstract class AttackOverTimeItemEffect : ItemEffect // 몬스터 지속 데미지 아이템
{
    public override void Execute(ItemUseContext itemUseContext)
    {
        if (itemUseContext.MonstersSyetem != null)
        {
            AttackOverTimeExecute(itemUseContext);
        }
    }

    public abstract void AttackOverTimeExecute(ItemUseContext itemUseContext);
}