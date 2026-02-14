using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSystem : MonoBehaviour, ISubSystem // 몬스터 시스템
{
    private GameManager gameManager;
    private PoolingSystem poolingSystem;
    private LevelSystem levelSystem;
    private HpSystem hpSystem;

    [SerializeField] private Monster[] monsterPrefabs;
    [SerializeField] private Transform monsterRoot;
    [SerializeField] private MonsterSystemUI monsterSystemUI;
    [SerializeField] private Canvas canvas;

    private List<Monster> monsters = new List<Monster>();
    private Vector2[] spawnPositons;

    private int poolSize = 20;
    private WaitForSeconds interval;
    private float breakTime = 10.0f;
    private Coroutine currentCoroutine;
    private Rect safeAreaRect;

    public List<Monster> Monsters => monsters;

    public void Start()
    {
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        safeAreaRect = SafeAreaUtil.GetSafeAreaInCanvas(canvasRect);
        float spawnHeight = safeAreaRect.yMax + 40;

        spawnPositons = new Vector2[] { new Vector2(-400, spawnHeight),
            new Vector2(-200, spawnHeight), new Vector2(0, spawnHeight),
            new Vector2(200, spawnHeight), new Vector2(400, spawnHeight) };
    }

    public void Initialize(GameManager gameManager)
    {
        this.gameManager = gameManager;
        poolingSystem = this.gameManager.SubSystemsManager.GetSubSystem<PoolingSystem>();
        levelSystem = this.gameManager.SubSystemsManager.GetSubSystem<LevelSystem>();
        hpSystem = this.gameManager.SubSystemsManager.GetSubSystem<HpSystem>();

        foreach (Monster monster in monsterPrefabs)
        {
            if (monster is BlueMonster blueMonster)
            {
                poolingSystem.CreatePool(blueMonster, monsterRoot, poolSize);
            }
            else if (monster is BugMonster bugMoster)
            {
                poolingSystem.CreatePool(bugMoster, monsterRoot, poolSize);
            }
            else if (monster is RedMonster redMoster)
            {
                poolingSystem.CreatePool(redMoster, monsterRoot, poolSize);
            }
        }

        currentCoroutine = StartCoroutine(WaveLoop());
    }

    public void Deinitialize()
    {
        StopCoroutine(currentCoroutine);
        currentCoroutine = null;
        interval = null;

        for (int i = monsterRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(monsterRoot.GetChild(i).gameObject);
        }
        monsters.Clear();

        gameManager = null;
        poolingSystem = null;
        levelSystem = null;
        hpSystem = null;
    }

    private IEnumerator WaveLoop()
    {
        while (spawnPositons == null || spawnPositons.Length == 0)
        {
            yield return null;
        }

        while (true)
        {
            yield return SpawnMonster();

            foreach (Monster monster in monsters)
            {
                monster.CurrentState = Monster.MonsterState.Die;
            }

            gameManager.InputLocked = true;
            monsterSystemUI.SetActive(true);

            float t = breakTime;
            while (t > 0)
            {
                t -= Time.deltaTime;
                int time = Mathf.CeilToInt(t);
                monsterSystemUI.SetTimer(time);

                yield return null;
            }

            monsterSystemUI.SetActive(false);
            levelSystem.LevelUp();
            gameManager.InputLocked = false;
        }
    }

    IEnumerator SpawnMonster()
    {
        float startTime = Time.time;

        float hpMul = levelSystem.GetMonsterHpMultiplier();
        float spawnMul = levelSystem.GetSpawnIntervalMultiplier();
        interval = new WaitForSeconds(5 * spawnMul);

        while (Time.time - startTime < 180f)
        {
            int monsterIdx = Random.Range(0, monsterPrefabs.Length);
            int spawnIdx = Random.Range(0, spawnPositons.Length);

            Monster monster = null;
            if (monsterPrefabs[monsterIdx] is BlueMonster)
            {
                monster = poolingSystem.GetPool<BlueMonster>();
                monster.Initialize(this, hpSystem, hpMul);
                monsters.Add(monster);
            }
            else if (monsterPrefabs[monsterIdx] is BugMonster)
            {
                monster = poolingSystem.GetPool<BugMonster>();
                monster.Initialize(this, hpSystem, hpMul);
                monsters.Add(monster);
            }
            else if (monsterPrefabs[monsterIdx] is RedMonster)
            {
                monster = poolingSystem.GetPool<RedMonster>();
                monster.Initialize(this, hpSystem, hpMul);
                monsters.Add(monster);
            }

            monster.Rect.anchoredPosition = spawnPositons[spawnIdx];

            yield return interval;
        }
    }

    public void ReturnToPool(Monster monster)
    {
        monster.gameObject.SetActive(false);
        if (monster is BlueMonster blueMonster)
        {
            poolingSystem.ReturnPool(blueMonster);
        }
        else if (monster is BugMonster BugMonster)
        {
            poolingSystem.ReturnPool(BugMonster);
        }
        else if (monster is RedMonster RedMonster)
        {
            poolingSystem.ReturnPool(RedMonster);
        }

        monsters.Remove(monster);
    }

    public bool IsPause()
    {
        if (gameManager == null)
            return false;

        return gameManager.IsPause;
    }
}
