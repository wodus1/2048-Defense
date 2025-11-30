using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class LevelSystem : MonoBehaviour, ISubSystem
{
    private int level = 1;
    private int exp = 0;
    private GameManager gameManager;

    public void Initialize(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }

    public void Deinitialize() { }

    public void AddExp(int amount)
    {
        exp += GetXpFromTile(amount);

        if (exp > 100)
        {
            exp -= 100;
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