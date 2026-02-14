using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsSystem : MonoBehaviour, ISubSystem // 플레이어 스탯 시스템
{
    private GameManager gameManager;
    [SerializeField] PlayerStatsUI playerStatsUI;

    private float normalDamage = 5f;
    private float normalAttackSpeed = 1f;
    private float normalProjectileSpeed = 900f;

    private List<(DamageMultiplierEffect effect, object handle)> damageMultiplierEffects= 
        new List<(DamageMultiplierEffect effect, object handle)>();
    private List<(AttackSpeedMultiplierEffect effect, object handle)> attackSpeedMultiplierEffects = 
        new List<(AttackSpeedMultiplierEffect effect, object handle)>();
    private List<(ProjectileSpeedMultiplierEffect effect, object handle)> projectileSpeedMultiplierEffects = 
        new List<(ProjectileSpeedMultiplierEffect effect, object handle)>();

    public Action<float> OnAttackSpeedChanged;
    public float NormalAttackSpeed => normalAttackSpeed;

    public void Initialize(GameManager gameManager)
    {
        this.gameManager = gameManager;
        playerStatsUI.Initialize(this);
    }

    public void Deinitialize()
    {
        gameManager = null;

        RemoveMultiplierEffects();
    }

    public float GetFinalDamage()
    {
        float finalDamage = normalDamage;
        foreach (var effect in damageMultiplierEffects)
            finalDamage *= effect.effect.Value;

        return finalDamage;
    }

    public float GetFinalAttackSpeed()
    {
        float finalAttackSpeed = normalAttackSpeed;
        foreach (var effect in attackSpeedMultiplierEffects)
            finalAttackSpeed *= effect.effect.Value;

        return finalAttackSpeed;
    }

    public float GetFinalProjectileSpeed()
    {
        float finalProjectileSpeed = normalProjectileSpeed;
        foreach (var effect in projectileSpeedMultiplierEffects)
            finalProjectileSpeed *= effect.effect.Value;

        return finalProjectileSpeed;
    }

    public void AddMultiplierEffect(Effect effect, object handle)
    {
        if (effect is DamageMultiplierEffect damageMultiplierEffect)
        {
            damageMultiplierEffects.Add((damageMultiplierEffect, handle));
        }
        else if (effect is AttackSpeedMultiplierEffect attackSpeedMultiplierEffect)
        {
            attackSpeedMultiplierEffects.Add((attackSpeedMultiplierEffect, handle));
            OnAttackSpeedChanged?.Invoke(GetFinalAttackSpeed());
        }
        else if (effect is ProjectileSpeedMultiplierEffect projectileSpeedMultiplierEffect)
        {
            projectileSpeedMultiplierEffects.Add((projectileSpeedMultiplierEffect, handle));
        }
    }

    public void RemoveMultiplierEffect(object handle)
    {
        damageMultiplierEffects.RemoveAll(e => e.handle == handle);
        attackSpeedMultiplierEffects.RemoveAll(e => e.handle == handle);
        projectileSpeedMultiplierEffects.RemoveAll(e => e.handle == handle);

        OnAttackSpeedChanged?.Invoke(GetFinalAttackSpeed());
    }

    private void RemoveMultiplierEffects()
    {
        damageMultiplierEffects.Clear();
        attackSpeedMultiplierEffects.Clear();
        projectileSpeedMultiplierEffects.Clear();
    }

    public void OnPlayerStatsUIStatsUI()
    {
        Pause();

        float clampAttackSpeed = Mathf.Clamp(GetFinalAttackSpeed(), normalAttackSpeed, 10f);
        playerStatsUI.SetStatsUI(GetFinalDamage(), clampAttackSpeed, GetFinalProjectileSpeed());
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