using UnityEngine;

[CreateAssetMenu(menuName = "Effect/ProjectileSpeedUp")]
public class ProjectileSpeedUpEffect : Effect //공격력 증가 효과
{
    public override void Apply(UpgradeSystem upgrade)
    {
        upgrade.AddProjectileSpeedMultiplier(value);
    }
}