using UnityEngine;

[CreateAssetMenu(menuName = "Effect/AttackSpeedUp")]
public class AttackSpeedUpEffect : Effect //공격 속도 증가 효과
{
    public override void Apply(UpgradeSystem upgradeSystem)
    {
        upgradeSystem.AddAttackSpeedMultiplier(value);
    }
}