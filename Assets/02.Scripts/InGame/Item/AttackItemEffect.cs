using UnityEngine;

public abstract class AttackItemEffect : ItemEffect // 공격형 아이템
{
    public override void Execute(ItemUseContext itemUseContext)
    {
        if(itemUseContext.MonstersSyetem != null)
        {
            AttackExecute(itemUseContext);
        }
    }

    public abstract void AttackExecute(ItemUseContext itemUseContext);
}