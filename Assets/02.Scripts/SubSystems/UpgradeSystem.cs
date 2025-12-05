using UnityEngine;
using System.Collections.Generic;

public class UpgradeSystem : MonoBehaviour, ISubSystem //강화 시스템
{
    private GameManager gameManger;
    [SerializeField] private List<Effect> Effects = new List<Effect>();
    private LevelSystem levelSystem;

    private float damageMultiplier = 1f;
    private float attackSpeedMultiplier = 1f;
    private float projectileSpeedMultiplier = 1f;

    public float DamageMultiplier => damageMultiplier;
    public float AttackSpeedMultiplier => attackSpeedMultiplier;
    public float ProjectileSpeedMultiplier => projectileSpeedMultiplier;
    public void Initialize(GameManager gameManager)
    {
        this.gameManger = gameManager;
        levelSystem =this.gameManger.SubSystemsManager.GetSubSystem<LevelSystem>();
    }

    public void Deinitialize()
    {
    }

    public void AddDamageMultiplier(float delta)
    {
        damageMultiplier += delta;
    }

    public void AddAttackSpeedMultiplier(float delta)
    {
        attackSpeedMultiplier += delta;
    }

    public void AddProjectileSpeedMultiplier(float delta)
    {
        projectileSpeedMultiplier += delta;
    }
}