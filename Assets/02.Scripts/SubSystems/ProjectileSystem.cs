using UnityEngine;
using System.Collections;

public class ProjectileSystem : MonoBehaviour, ISubSystem //투사체 시스템
{
    private GameManager gameManager;
    private PoolingSystem poolingSystem;
    private MonsterSystem monsterSystem;
    private UpgradeSystem upgradeSystem;

    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Transform projectileRoot;
    
    private WaitForSeconds waitForSeconds;
    private int poolSize = 30;
    private Coroutine currentCoroutine;
    private float damage = 5;
    private float projectileSpeed = 900f;
    private float attackSpeed = 1f;
    private float currentAttackSpeed;

    public void Initialize(GameManager gameManager)
    {
        this.gameManager = gameManager;
        poolSize = 30;
        damage = 5;
        projectileSpeed = 900f;
        attackSpeed = 1f;

        poolingSystem = this.gameManager.SubSystemsManager.GetSubSystem<PoolingSystem>();
        monsterSystem = this.gameManager.SubSystemsManager.GetSubSystem<MonsterSystem>();
        upgradeSystem = this.gameManager.SubSystemsManager.GetSubSystem<UpgradeSystem>();

        currentAttackSpeed = attackSpeed * upgradeSystem.AttackSpeedMultiplier;
        waitForSeconds = new WaitForSeconds(currentAttackSpeed);
        poolingSystem.CreatePool(projectilePrefab, projectileRoot, poolSize);
        currentCoroutine = StartCoroutine(ShotLogic());
    }

    public void Deinitialize()
    {
        StopCoroutine(currentCoroutine);
        currentCoroutine = null;

        gameManager = null;
        poolingSystem = null;
        monsterSystem = null;
        upgradeSystem = null;
    }

    private IEnumerator ShotLogic()
    {
        while (true)
        {
            float attackSpeed = this.attackSpeed * upgradeSystem.AttackSpeedMultiplier;
            float newAttackSpeed = Mathf.Clamp(attackSpeed, 0.1f, float.MaxValue);

            if (!Mathf.Approximately(currentAttackSpeed, newAttackSpeed))
            {
                currentAttackSpeed = newAttackSpeed;
                waitForSeconds = new WaitForSeconds(currentAttackSpeed);
            }

            yield return waitForSeconds;

            if (monsterSystem.Monsters == null || monsterSystem.Monsters.Count < 1)
                continue;

            if (!TryGetNearMonsterPos(out Vector3 targetPos))
                continue;

            var projectile = poolingSystem.GetPool<Projectile>();
            projectile.Initialize(this);

            float finalDamage = damage * upgradeSystem.DamageMultiplier;
            float finalSpeed = projectileSpeed * upgradeSystem.ProjectileSpeedMultiplier;

            projectile.Shoot(projectileRoot.position, targetPos, finalDamage, finalSpeed);
        }
    }

    public void ReturnToPool(Projectile projectile)
    {
        projectile.gameObject.SetActive(false);
        poolingSystem.ReturnPool(projectile);
    }

    private bool TryGetNearMonsterPos(out Vector3 targetPos)
    {
        Vector3 myPos = projectileRoot.position;
        targetPos = myPos;
        float nearVertical = float.MaxValue;
        bool found = false;

        foreach (var monster in monsterSystem.Monsters)
        {
            if (!monster.IsVisible())
                break;

            if (monster.CurrentState == Monster.MonsterState.Die)
                continue;

            float verticalDiff = Mathf.Abs(monster.transform.position.y - myPos.y);

            if (verticalDiff < nearVertical)
            {
                nearVertical = verticalDiff;
                targetPos = monster.transform.position;
                found = true;
            }
        }

        return found;
    }

    public bool IsPause()
    {
        if (gameManager == null)
            return false;

        return gameManager.IsPause;
    }
}