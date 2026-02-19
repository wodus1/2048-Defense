using System;
using System.Collections.Generic;

[Serializable]
public class PlayerData // 유저 데이터 모델
{
    public int gold;
    public Dictionary<string,int> items = new Dictionary<string,int>();
    public int revision; // 서버 revision
    public long lastSavedAtUnix; // 마지막 저장 성공 시간

    [NonSerialized] public Action OnChanged;

    public void AddGold(int amount)
    {
        if (amount <= 0) return;
        gold += amount;

        OnChanged?.Invoke();
    }

    public bool TryConsumeGold(int amount)
    {
        if (amount <= 0) return true;
        if (gold < amount) return false;
        gold -= amount;

        OnChanged?.Invoke();
        return true;
    }

    public int GetItemCount(string itemId)
    {
        if(items.TryGetValue(itemId, out var cnt))
            return cnt;

        return 0;
    }

    public void AddItem(string itemId, int amount)
    {
        if (string.IsNullOrEmpty(itemId) || amount <= 0) return;

        items.TryGetValue(itemId, out int cur);
        items[itemId] = cur + amount;
        
        OnChanged?.Invoke();
    }

    public bool TryConsumeItem(string itemId, int amount)
    {
        if (string.IsNullOrEmpty(itemId)) return false;
        if (amount <= 0) return true;

        items.TryGetValue(itemId, out int cur);
        if (cur < amount) return false;

        int next = cur - amount;
        if (next == 0) items.Remove(itemId);
        else items[itemId] = next;

        OnChanged?.Invoke();
        return true;
    }
}
