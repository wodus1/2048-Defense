using UnityEngine;

[CreateAssetMenu(menuName = "Effect/ProjectileSpeedUp")]
public class ProjectileSpeedUpEffect : Effect //투사체 속도 증가 효과
{
    public override void Apply(UpgradeSystem upgradeSystem)
    {
        upgradeSystem.AddProjectileSpeedMultiplier(value);
    }
}