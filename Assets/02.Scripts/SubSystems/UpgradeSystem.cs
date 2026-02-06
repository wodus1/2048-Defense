using UnityEngine;
using System.Collections.Generic;

public class UpgradeSystem : MonoBehaviour, ISubSystem //강화 시스템
{
    private GameManager gameManager;
    private LevelSystem levelSystem;
    private PlayerStatsSystem playerStatsSystem;

    [SerializeField] private List<Effect> effects = new List<Effect>();
    [SerializeField] private UpgradeUI upgradeUI;

    public PlayerStatsSystem PlayerStatsSystem => playerStatsSystem;

    public void Initialize(GameManager gameManager)
    {
        this.gameManager = gameManager;
        playerStatsSystem = this.gameManager.SubSystemsManager.GetSubSystem<PlayerStatsSystem>();
        levelSystem = this.gameManager.SubSystemsManager.GetSubSystem<LevelSystem>();
        levelSystem.OnUpgrade += GetRandomEffects;
    }

    public void Deinitialize()
    {
        gameManager = null;

        levelSystem.OnUpgrade -= GetRandomEffects;
        levelSystem = null;
    }

    private void GetRandomEffects()
    {
        int firstIndex = Random.Range(0, effects.Count);
        Effect firstEffect = effects[firstIndex];

        int secondIndex = firstIndex;
        while (secondIndex == firstIndex)
        {
            secondIndex = Random.Range(0, effects.Count);
        }

        Effect secondEffect = effects[secondIndex];

        upgradeUI.Initialize(this, firstEffect, secondEffect);
    }

    public void Resume()
    {
        if (gameManager == null)
            return;

        gameManager.Resume();
    }

    public void Pause()
    {
        if (gameManager == null)
            return;

        gameManager.Pause();
    }
}