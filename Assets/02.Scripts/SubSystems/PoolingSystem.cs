using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolingSystem : MonoBehaviour, ISubSystem // 풀링 시스템
{
    private GameManager gameManger;
    private Dictionary<Type, IObjectPool> pools = new Dictionary<Type, IObjectPool>();
    private Dictionary<ParticleSystem, IObjectPool> fxPools = new Dictionary<ParticleSystem, IObjectPool>();

    public void Initialize(GameManager gameManager)
    {
        this.gameManger = gameManager;
    }

    public void Deinitialize()
    {
        foreach (var poolType in pools.Values)
            poolType.ClearPool();

        foreach (var fxPool in pools.Values)
            fxPool.ClearPool();

        pools.Clear();
        fxPools.Clear();

        gameManger = null;
    }

    #region 타입 오브젝트 풀
    public void CreatePool<T>(T prefab, Transform root, int size) where T : Component
    {
        var type = typeof(T);
        if (pools.ContainsKey(type))
            return;

        pools[type] = new ObjectPool<T>(prefab, root, size);
    }

    public T GetPool<T>() where T : Component
    {
        if (pools.TryGetValue(typeof(T), out var poolType) &&
            poolType is ObjectPool<T> pool)
        {
            return pool.GetPool();
        }
        
        return null;
    }

    public void ReturnPool<T>(T obj) where T : Component
    {
        var type = typeof(T);

        if (pools.TryGetValue(type, out var poolType) &&
            poolType is ObjectPool<T> pool)
        {
            pool.ReturnPool(obj);
        }
        else
        {
            Destroy(obj.gameObject);
        }
    }

    public bool poolsContains<T>() where T : Component
    {
        return pools.ContainsKey(typeof(T));
    }
    #endregion

    #region fx 오브젝트 풀
    public void CreateFXPool(ParticleSystem fx, Transform root, int size)
    {
        if (fxPools.ContainsKey(fx)) return;

        fxPools[fx] = new ObjectPool<ParticleSystem>(fx, root, size);
    }

    public ParticleSystem GetFXPool(ParticleSystem fx)
    {
        if (fxPools.TryGetValue(fx, out var poolType) &&
            poolType is ObjectPool<ParticleSystem> pool)
        {
            return pool.GetPool();
        }

        return null;
    }

    public void ReturnFXPool(ParticleSystem fx, ParticleSystem obj)
    {
        if (fxPools.TryGetValue(fx, out var poolType) &&
            poolType is ObjectPool<ParticleSystem> pool)
        {
            pool.ReturnPool(obj);
        }
        else
        {
            Destroy(obj.gameObject);
        }
    }

    public bool fxPoolsContains(ParticleSystem fx)
    {
        return fxPools.ContainsKey(fx);
    }
    #endregion
}