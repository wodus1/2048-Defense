using UnityEngine;

[CreateAssetMenu(menuName = "Effect/AttackSpeedMultiplier")]
public class AttackSpeedMultiplierEffect : Effect // 공격 속도 증가 효과
{
    [SerializeField] private float value = 0.2f;
    public float Value => value;
    
    public override void Apply(PlayerStatsSystem playerStatsSystem, object handle)
    {
        playerStatsSystem.AddMultiplierEffect(this, handle);
    }
}