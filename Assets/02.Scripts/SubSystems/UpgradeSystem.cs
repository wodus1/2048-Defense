using UnityEngine;
using System.Collections.Generic;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;
using UnityEngine.InputSystem.Controls;

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
        levelSystem =this.gameManger.SubSystemsManager.GetSubSystem<LevelSystem>();
        levelSystem.OnUpgrade += OnUpgrade;
    }

    public void Deinitialize()
    {
        effects.Clear();
    }

    private void GetRandomEffects(out Effect effect1, out Effect effect2)
    {
        int firstIndex = Random.Range(0, effects.Count);
        effect1 = effects[firstIndex];

        int secondIndex = firstIndex;
        while (secondIndex == firstIndex)
        {
            secondIndex = Random.Range(0, effects.Count);
        }

        effect2 = effects[secondIndex];

        upgradeUI.Initialize(this, effect1, effect2);
    }

    private void OnUpgrade()
    {
        GetRandomEffects(out Effect effect1, out Effect effect2);
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
}