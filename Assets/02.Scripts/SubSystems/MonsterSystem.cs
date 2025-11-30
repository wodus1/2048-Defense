using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSystem : MonoBehaviour, ISubSystem //몬스터 시스템
{
    private GameManager gameManager;
    private PoolingSystem poolingSystem;
    [SerializeField] private Monster[] monsterPrefabs;
    [SerializeField] private Transform monsterRoot;
    private List<Monster> monsters = new List<Monster>();
    private List<Vector2> spawnPositons = new List<Vector2>()
    { new Vector2(-400, 1040), new Vector2(-200, 1040), new Vector2(0, 1040), new Vector2(200, 1040), new Vector2(400, 1040) };
    private int poolSize = 20;
    private WaitForSeconds waitForSeconds;
    private Coroutine currentCoroutine;

    public List<Monster> Monsters => monsters;

    public void Initialize(GameManager gameManager)
    {
        this.gameManager = gameManager;
        poolingSystem = this.gameManager.SubSystemsManager.GetSubSystem<PoolingSystem>();
        waitForSeconds = new WaitForSeconds(4.0f);
        
        foreach(Monster monster in monsterPrefabs)
        {
            if (monster is BlueMonster blueMonster)
            {
                poolingSystem.CreatePool(blueMonster, monsterRoot, poolSize);
            }
        }

        currentCoroutine = StartCoroutine(CreateMonster());
    }

    public void Deinitialize()
    {
        StopCoroutine(currentCoroutine);
        currentCoroutine = null;
    }

    IEnumerator CreateMonster()
    {
        while (true)
        {
            int monsterIdx = Random.Range(0, monsterPrefabs.Length);
            int spawnIdx = Random.Range(0, spawnPositons.Count);
            
            Monster monster = null;
            if (monsterPrefabs[monsterIdx] is BlueMonster)
            {
                monster = poolingSystem.GetPool<BlueMonster>();
                monster.Initialize(this);
                monsters.Add(monster);
            }

            monster.Rect.anchoredPosition = spawnPositons[spawnIdx];

            yield return waitForSeconds;
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
