using UnityEngine;

[CreateAssetMenu(menuName = "Effect/DamageUp")]
public class DamageUpEffect : Effect //공격력 증가 효과
{
    public override void Apply(UpgradeSystem upgradeSystem)
    {
        upgradeSystem.AddDamageMultiplier(value);
    }
}