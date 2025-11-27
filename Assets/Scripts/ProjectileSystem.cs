using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProjectileSystem : MonoBehaviour, ISubSystem
{
    private GameManager gameManager;
    private PoolingSystem poolingSystem;
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Transform projectileRoot;
    
    private WaitForSeconds waitForSeconds;
    private int poolSize = 30;
    
    public void Initialize(GameManager gameManager)
    {
        this.gameManager = gameManager;
        poolingSystem = this.gameManager.SubSystemsManager.GetSubSystem<PoolingSystem>();
        waitForSeconds = new WaitForSeconds(1.0f);
        poolingSystem.CreatePool<Projectile>(projectilePrefab, projectileRoot,poolSize);
        StartCoroutine(ShotLogic());
    }

    public void Deinitialize()
    { 
    }

    private IEnumerator ShotLogic()
    {
        while (true)
        {
            foreach (TileUI tile in gameManager.Tiles)
            {
                if (tile.Value > 0)
                {
                    var projectile = poolingSystem.GetPool<Projectile>();
                    projectile.Initialize(this);
                    projectile.Shoot(tile.transform.position, new Vector2(tile.transform.position.x, tile.transform.position.y+900));
                }
            }
            
            yield return waitForSeconds;
        }
    }

    public void ReturnToPool(Projectile projectile)
    {
        projectile.gameObject.SetActive(false);
        poolingSystem.ReturnPool(projectile);
    }
}
