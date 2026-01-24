using UnityEngine;
using System.Collections;

public class ProjectileSystem : MonoBehaviour, ISubSystem //투사체 시스템
{
    private GameManager gameManager;
    private PoolingSystem poolingSystem;
    private MonsterSystem monsterSystem;
    private PlayerStatsSystem playerStatsSystem;

    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Transform projectileRoot;
    
    private WaitForSeconds waitForSeconds;
    private int poolSize = 30;
    private Coroutine currentCoroutine;

    public void Initialize(GameManager gameManager)
    {
        this.gameManager = gameManager;
        poolSize = 30;

        poolingSystem = this.gameManager.SubSystemsManager.GetSubSystem<PoolingSystem>();
        monsterSystem = this.gameManager.SubSystemsManager.GetSubSystem<MonsterSystem>();
        playerStatsSystem = this.gameManager.SubSystemsManager.GetSubSystem<PlayerStatsSystem>();

        waitForSeconds = new WaitForSeconds(playerStatsSystem.GetFinalAttackSpeed());
        poolingSystem.CreatePool(projectilePrefab, projectileRoot, poolSize);
        currentCoroutine = StartCoroutine(ShotLogic());

        playerStatsSystem.OnAttackSpeedChanged += UpdateAttackSpeed;
    }

    public void Deinitialize()
    {
        StopCoroutine(currentCoroutine);
        currentCoroutine = null;

        gameManager = null;
        poolingSystem = null;
        monsterSystem = null;
        playerStatsSystem = null;

        playerStatsSystem.OnAttackSpeedChanged -= UpdateAttackSpeed;
    }

    private IEnumerator ShotLogic()
    {
        while (true)
        {
            yield return waitForSeconds;

            if (monsterSystem.Monsters == null || monsterSystem.Monsters.Count < 1)
                continue;

            if (!TryGetNearMonsterPos(out Vector3 targetPos))
                continue;

            var projectile = poolingSystem.GetPool<Projectile>();
            projectile.Initialize(this);
            projectile.Shoot(projectileRoot.position, targetPos, 
                playerStatsSystem.GetFinalAttackSpeed(), playerStatsSystem.GetFinalProjectileSpeed());
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

    private void UpdateAttackSpeed(float newAttackSpeed)
    {
        float clampedAttackSpeed = Mathf.Clamp(newAttackSpeed, playerStatsSystem.NormalAttackSpeed, 10f);
        waitForSeconds = new WaitForSeconds(1f / clampedAttackSpeed);
    }
}