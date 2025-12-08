using UnityEngine;
using System.Collections.Generic;

public class UpgradeSystem : MonoBehaviour, ISubSystem //강화 시스템
{
    private GameManager gameManger;
    private LevelSystem levelSystem;

    [SerializeField] private List<Effect> effects = new List<Effect>();
    [SerializeField] private UpgradeUI upgradeUI;

    private float damageMultiplier = 1f;
    private float attackSpeedMultiplier = 1f;
    private float projectileSpeedMultiplier = 1f;
    private float attackSpeed = 1f;

    public float DamageMultiplier => damageMultiplier;
    public float AttackSpeedMultiplier => attackSpeedMultiplier;
    public float ProjectileSpeedMultiplier => projectileSpeedMultiplier;
    public void Initialize(GameManager gameManager)
    {
        this.gameManger = gameManager;
        damageMultiplier = 1f;
        attackSpeedMultiplier = 1f;
        projectileSpeedMultiplier = 1f;
        attackSpeed = 1f;

        levelSystem = this.gameManger.SubSystemsManager.GetSubSystem<LevelSystem>();
        levelSystem.OnUpgrade += GetRandomEffects;
    }

    public void Deinitialize()
    {
        effects.Clear();
        gameManger = null;
        levelSystem = null;
    }

    private void GetRandomEffects()
    {
        int firstIndex = Random.Range(0, effects.Count);
        Effect effect1 = effects[firstIndex];

        int secondIndex = firstIndex;
        while (secondIndex == firstIndex)
        {
            secondIndex = Random.Range(0, effects.Count);
        }

        Effect effect2 = effects[secondIndex];

        upgradeUI.Initialize(this, effect1, effect2);
    }

    public void AddDamageMultiplier(float delta)
    {
        damageMultiplier += delta * 0.01f;
    }

    public void AddAttackSpeedMultiplier(float delta)
    {
        attackSpeed += delta * 0.01f;
        attackSpeedMultiplier = 1 / attackSpeed;
    }

    public void AddProjectileSpeedMultiplier(float delta)
    {
        projectileSpeedMultiplier += delta * 0.01f;
    }

    public void Resume()
    {
        gameManger.Resume();
    }

    public void Pause()
    {
        gameManger.Pause();
    }
}