using System;
using UnityEngine;

public class LevelSystem : MonoBehaviour, ISubSystem //레벨업 시스템
{
    private GameManager gameManager;
    private int level = 1;
    private int exp = 0;

    public event Action OnUpgrade;

    public void Initialize(GameManager gameManager)
    {
        this.gameManager = gameManager;
        level = 1;
        exp = 0;
    }

    public void Deinitialize()
    {
        OnUpgrade = null;
        gameManager = null;
    }

    public void AddExp(int amount)
    {
        exp += GetXpFromTile(amount);

        if (exp > 100)
        {
            exp -= 100;
            OnUpgrade?.Invoke();
        }
    }

    public float GetMonsterHpMultiplier()
    {
        return 1 + (level - 1) * 0.25f;
    }

    public float GetSpawnIntervalMultiplier()
    {
        float mul = 1 - (level - 1) * 0.07f;
        return Mathf.Max(0.1f, mul);
    }

    public void LevelUp()
    {
        level++;
        gameManager.ClearTilesExceptMax();
    }

    private int GetXpFromTile(int value)
    {
        if (value <= 64)
            return value / 2;

        if (value == 128)
            return 50;

        switch (value)
        {
            case 256:
                return 65;
            case 512: 
                return 80;
            case 1024:
                return 90;
            case 2048: 
                return 100;
            default:
                return 100;
        }
    }
}