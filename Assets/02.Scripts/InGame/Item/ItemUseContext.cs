using UnityEngine;

public sealed class ItemUseContext // 아이템 사용 시 필요한 컨텍스트
{
    public PlayerStatsSystem PlayerStatsSystem { get; }
    public MonsterSystem MonstersSyetem { get; }
    public FXSystem FXSystem { get; }
    public object Handle { get; }

    public ItemUseContext(PlayerStatsSystem playerStats, MonsterSystem monsters, FXSystem fx, object handle)
    {
        PlayerStatsSystem = playerStats;
        MonstersSyetem = monsters;
        FXSystem = fx;
        Handle = handle;
    }
}