using UnityEngine;

[CreateAssetMenu(menuName = "Effect/DamageMultiplier")]
public class DamageMultiplierEffect : Effect //공격력 증가 효과
{
    [SerializeField] private float value = 0.2f;
    public float Value => value;

    public override void Apply(PlayerStatsSystem playerStatsSystem, object source)
    {
        playerStatsSystem.AddMultiplierEffect(this, source);
    }
}