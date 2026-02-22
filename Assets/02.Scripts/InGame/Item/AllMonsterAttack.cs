using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Attack/AllAttack")]
public class AllMonsterAttackItem : AttackItemEffect // 필드에 있는 모든 몬스터 공격
{
    [SerializeField] private float damage = 10;
    
    public override void AttackExecute(ItemUseContext itemUseContext)
    {
        MonsterSystem monsterSystem = itemUseContext.MonstersSyetem;
        FXSystem fxSystem = itemUseContext.FXSystem;
        var monsterList = monsterSystem.Monsters.ToList();

        if (Particle != null && attackType == AttackType.Area)
        {
            RectTransform fxRect = monsterSystem.canvas.GetComponent<RectTransform>();
            fxSystem.Play(Particle, fxRect, new Vector2(0, fxRect.rect.height * 0.25f), Quaternion.Euler(-90f, 0f, 0f));
        }

        foreach (var monster in monsterList)
        {
            if (monster == null) continue;

            if (Particle != null && attackType == AttackType.Normal)
                fxSystem.Play(Particle, monster.Rect, new Vector2(0,0), Quaternion.identity);

            monster.TakeDamage(damage);
        }
    }
}