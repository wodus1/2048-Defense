using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static Monster;

public class ProjectileSystem : MonoBehaviour, ISubSystem //투사체 시스템
{
    private GameManager gameManager;
    private PoolingSystem poolingSystem;
    private MonsterSystem monsterSystem;
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Transform projectileRoot;
    
    private WaitForSeconds waitForSeconds;
    private int poolSize = 30;
    private Coroutine currentCoroutine;
    private float damage = 5;
    private float speed = 900f;

    public void Initialize(GameManager gameManager)
    {
        this.gameManager = gameManager;
        poolingSystem = this.gameManager.SubSystemsManager.GetSubSystem<PoolingSystem>();
        monsterSystem = this.gameManager.SubSystemsManager.GetSubSystem<MonsterSystem>();
        waitForSeconds = new WaitForSeconds(1.0f);
        poolingSystem.CreatePool(projectilePrefab, projectileRoot, poolSize);
        currentCoroutine = StartCoroutine(ShotLogic());
    }

    public void Deinitialize()
    {
        StopCoroutine(currentCoroutine);
        currentCoroutine = null;
    }

    private IEnumerator ShotLogic()
    {
        while (true)
        {
            yield return waitForSeconds;

            if (monsterSystem.Monsters.Count < 1 || monsterSystem.Monsters == null)
                continue;

            if (!TryGetNearMonsterPos(out Vector3 targetPos))
                continue;

            var projectile = poolingSystem.GetPool<Projectile>();
            projectile.Initialize(this);
            projectile.Shoot(projectileRoot.position, targetPos, damage, speed);
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

            if (monster.CurrentState == MonsterState.Die)
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
}
