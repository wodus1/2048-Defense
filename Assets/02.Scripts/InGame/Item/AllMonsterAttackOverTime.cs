using System.Collections;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Attack/AllAttackOverTime")]
public class AllMonsterAttackOverTime : AttackOverTimeItemEffect // 모든 몬스터에 일정시간 지속 데미지
{
    [SerializeField] private float damage = 3f;
    [SerializeField] private float duration = 5f;
    [SerializeField] private float interval = 1f;

    public override void AttackOverTimeExecute(ItemUseContext itemUseContext)
    {
        if(itemUseContext.MonstersSyetem != null)
            itemUseContext.MonstersSyetem.StartCoroutine(AttackExecute(itemUseContext));
    }

    private IEnumerator AttackExecute(ItemUseContext itemUseContext)
    {
        MonsterSystem monsterSystem = itemUseContext.MonstersSyetem;
        WaitForSeconds wfs = new WaitForSeconds(interval);
        float duration = this.duration;

        while(duration > 0)
        {
            var monsterList = monsterSystem.Monsters.ToList();
            foreach (var monster in monsterList)
            {
                if (monster == null) continue;

                monster.TakeDamage(damage);
            }

            duration -= 1;
            yield return null;
        }
    }
}
