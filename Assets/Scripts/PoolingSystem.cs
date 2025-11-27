using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolingSystem : MonoBehaviour, ISubSystem
{
    private GameManager gameManger;
    private Dictionary<Type, IObjectPool> pools = new Dictionary<Type, IObjectPool>();

    public void Initialize(GameManager gameManager)
    {
        this.gameManger = gameManager;
    }

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

    public void Deinitialize()
    {
        foreach (var poolType in pools.Values)
        {
            poolType.ClearPool();
        }

        pools.Clear();
    }
}