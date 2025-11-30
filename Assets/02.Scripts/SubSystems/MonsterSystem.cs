using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSystem : MonoBehaviour, ISubSystem //���� �ý���
{
    private GameManager gameManager;
    private PoolingSystem poolingSystem;
    private LevelSystem levelSystem;

    [SerializeField] private Monster[] monsterPrefabs;
    [SerializeField] private Transform monsterRoot;

    private List<Monster> monsters = new List<Monster>();
    private List<Vector2> spawnPositons = new List<Vector2>()
    { new Vector2(-400, 1040), new Vector2(-200, 1040), new Vector2(0, 1040), new Vector2(200, 1040), new Vector2(400, 1040) };

    private int poolSize = 20;
    private WaitForSeconds interval = new WaitForSeconds(5.0f);
    private WaitForSeconds breakTime = new WaitForSeconds(10.0f);
    private Coroutine currentCoroutine;

    public List<Monster> Monsters => monsters;

    public void Initialize(GameManager gameManager)
    {
        this.gameManager = gameManager;
        poolingSystem = this.gameManager.SubSystemsManager.GetSubSystem<PoolingSystem>();
        levelSystem = this.gameManager.SubSystemsManager.GetSubSystem<LevelSystem>();

        foreach (Monster monster in monsterPrefabs)
        {
            if (monster is BlueMonster blueMonster)
            {
                poolingSystem.CreatePool(blueMonster, monsterRoot, poolSize);
            }
        }

        currentCoroutine = StartCoroutine(WaveLoop());
    }

    public void Deinitialize()
    {
        StopCoroutine(currentCoroutine);
        currentCoroutine = null;
        monsters.Clear();
    }

    private IEnumerator WaveLoop()
    {
        while (true)
        {
            yield return SpawnMonster();
            
            foreach(Monster monster in monsters)
            {
                monster.CurrentState = Monster.MonsterState.Die;
            }

            yield return breakTime;

            levelSystem.LevelUp();
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
            int spawnIdx = Random.Range(0, spawnPositons.Count);
            
            Monster monster = null;
            if (monsterPrefabs[monsterIdx] is BlueMonster)
            {
                monster = poolingSystem.GetPool<BlueMonster>();
                monster.Initialize(this, hpMul);
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

        monsters.Remove(monster);
    }
}
