using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Attack/AllAttack")]
public class AllMonsterAttackItem : AttackItemEffect // 필드에 있는 모든 몬스터 공격
{
    [SerializeField] private float damage = 10;
    
    public override void AttackExecute(ItemUseContext itemUseContext)
    {
        MonsterSystem monsterSystem = itemUseContext.MonstersSyetem;
        var monsterList = monsterSystem.Monsters.ToList();

        foreach (var monster in monsterList)
        {
            if (monster == null) continue;

            monster.TakeDamage(damage);
        }
    }
}
