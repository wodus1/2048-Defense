using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProjectileSystem : MonoBehaviour, ISubSystem
{
    private Game2048Manager game2048Manager;
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Transform projectileRoot;
    
    private WaitForSeconds waitForSeconds;
    private Stack<Projectile> pool = new Stack<Projectile>();
    private int poolSize = 30;
    
    public void Initialize(Game2048Manager game2048Manager)
    {
        this.game2048Manager = game2048Manager;
        waitForSeconds = new WaitForSeconds(1.0f);
        
        for (int i = 0; i < poolSize; i++)
        {
            var projectile = Instantiate(projectilePrefab, projectileRoot);
            projectile.gameObject.SetActive(false);
            projectile.Initialize(this);
            pool.Push(projectile);
        }

        StartCoroutine(ShotLogic());
    }

    private IEnumerator ShotLogic()
    {
        while (true)
        {
            foreach (TileUI tile in game2048Manager.Tiles)
            {
                if (tile.Value > 0)
                {
                    var projectile = GetProjectile();
                    projectile.Shoot(tile.transform.position, new Vector2(tile.transform.position.x, tile.transform.position.y+900));
                }
            }
            
            yield return waitForSeconds;
        }
    }

    private Projectile GetProjectile()
    {
        Projectile projectile;

        if (pool.Count > 0)
        {
            projectile = pool.Pop();
        }
        else
        {
            projectile = Instantiate(projectilePrefab, projectileRoot);
            projectile.gameObject.SetActive(false);
            projectile.Initialize(this);
        }
        
        return projectile;
    }

    public void ReturnToPool(Projectile projectile)
    {
        projectile.gameObject.SetActive(false);
        pool.Push(projectile);
    }
}
