using UnityEngine;

public sealed class ItemUseContext // 아이템 사용 시 필요한 컨텍스트
{
    public PlayerStatsSystem PlayerStatsSystem { get; }
    public MonsterSystem MonstersSyetem { get; }
    public object Handle { get; }

    public ItemUseContext(PlayerStatsSystem playerStats, MonsterSystem monsters, Transform userTransform, object handle)
    {
        PlayerStatsSystem = playerStats;
        MonstersSyetem = monsters;
        Handle = handle;
    }
}