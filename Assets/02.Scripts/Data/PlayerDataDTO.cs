using System;
using System.Collections.Generic;


[Serializable]
public class PlayerDataDTO // PlayerData DTO 변환
{
    public int gold;
    public List<ItemPair> items = new List<ItemPair>();
    public int revision;
    public long lastSavedAtUnix;
    public string[] equipped = new string[3];

    [Serializable]
    public class ItemPair
    {
        public string id;
        public int count;
    }

    public static PlayerDataDTO From(PlayerData playerData)
    {
        var dto = new PlayerDataDTO
        {
            gold = playerData.gold,
            revision = playerData.revision,
            lastSavedAtUnix = playerData.lastSavedAtUnix,
            equipped = (string[])playerData.equipped.Clone()
        };

        foreach (var item in playerData.items)
            dto.items.Add(new ItemPair { id = item.Key, count = item.Value });

        return dto;
    }

    public PlayerData ToModel()
    {
        var data = new PlayerData
        {
            gold = gold,
            revision = revision,
            lastSavedAtUnix = lastSavedAtUnix,
            items = new Dictionary<string, int>(),
            equipped = (string[])equipped.Clone()
        };

        if (items != null)
        {
            foreach (var item in items)
            {
                if (string.IsNullOrEmpty(item.id)) continue;
                if (item.count <= 0) continue;
                data.items[item.id] = item.count;
            }
        }

        return data;
    }
}
