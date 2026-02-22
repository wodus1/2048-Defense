using UnityEngine;

public enum AttackType
{
    Normal,
    Area
}

public abstract class AttackItemEffect : ItemEffect // 몬스터 공격 아이템
{
    [SerializeField] protected AttackType attackType;

    public override void Execute(ItemUseContext itemUseContext)
    {
        if(itemUseContext.MonstersSyetem != null)
        {
            AttackExecute(itemUseContext);
        }
    }

    public abstract void AttackExecute(ItemUseContext itemUseContext);
}