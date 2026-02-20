using System;
using System.Collections.Generic;

[Serializable]
public class PlayerData // 유저 데이터 모델
{
    public int gold;
    public Dictionary<string,int> items = new Dictionary<string,int>();
    public int revision; // 서버 revision
    public long lastSavedAtUnix; // 마지막 저장 성공 시간
    public string[] equipped = new string[3];

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

    public int GetItemCount(string itemTitle)
    {
        if(items.TryGetValue(itemTitle, out var cnt))
            return cnt;

        return 0;
    }

    public void AddItem(string itemTitle, int amount)
    {
        if (string.IsNullOrEmpty(itemTitle) || amount <= 0) return;

        items.TryGetValue(itemTitle, out int cur);
        items[itemTitle] = cur + amount;
        
        OnChanged?.Invoke();
    }

    public bool TryConsumeItem(string itemTitle, int amount)
    {
        if (string.IsNullOrEmpty(itemTitle)) return false;
        if (amount <= 0) return true;

        items.TryGetValue(itemTitle, out int cur);
        if (cur < amount) return false;

        int next = cur - amount;
        if (next == 0) items.Remove(itemTitle);
        else items[itemTitle] = next;

        OnChanged?.Invoke();
        return true;
    }

    public void EquipItem(string itemTitle, int index)
    {
        if(string.IsNullOrEmpty(itemTitle)) return;
        if(index < 0 || index >= equipped.Length) return;
        equipped[index] = itemTitle;

        OnChanged?.Invoke();
    }

    public void UnequipItem(int index)
    {
        if (index < 0 || index >= equipped.Length) return;
        equipped[index] = null;

        OnChanged?.Invoke();
    }
}
