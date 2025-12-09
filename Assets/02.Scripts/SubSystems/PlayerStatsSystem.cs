using UnityEngine;

public class PlayerStatsSystem : MonoBehaviour, ISubSystem //플레이어 스탯 시스템
{
    private GameManager gameManager;
    [SerializeField] PlayerStatsUI playerStatsUI;

    private float normalDamage = 5f;
    private float normalAttackSpeed = 1f;
    private float normalProjectileSpeed = 900f;

    private float damageMultiplier = 1f;
    private float attackSpeed = 1f;
    private float attackSpeedMultiplier = 1f;
    private float projectileSpeedMultiplier = 1f;

    public float FinalDamage => normalDamage * damageMultiplier;
    public float FinalAttackSpeed => normalAttackSpeed * attackSpeedMultiplier;
    public float FinalProjectileSpeed => normalProjectileSpeed * projectileSpeedMultiplier;

    public void Initialize(GameManager gameManager)
    {
        this.gameManager = gameManager;
        playerStatsUI.Initialize(this);

        damageMultiplier = 1f;
        attackSpeedMultiplier = 1f;
        projectileSpeedMultiplier = 1f;
        attackSpeed = 1f;
    }

    public void Deinitialize()
    {
        gameManager = null;
    }

    public void AddDamageMultiplier(float delta)
    {
        damageMultiplier += delta * 0.01f;
    }

    public void AddAttackSpeedMultiplier(float delta)
    {
        attackSpeed += delta * 0.01f;

        attackSpeedMultiplier = 1f / attackSpeed;
    }

    public void AddProjectileSpeedMultiplier(float delta)
    {
        projectileSpeedMultiplier += delta * 0.01f;
    }

    public void OnPlayerStatsUIStatsUI()
    {
        Pause();
        playerStatsUI.SetStatsUI(FinalDamage, FinalAttackSpeed, FinalProjectileSpeed);
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