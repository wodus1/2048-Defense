using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public interface IObjectPool
{
    void ClearPool();
}

public class ObjectPool<T> : IObjectPool where T : Component
{
    private Stack<T> stack = new Stack<T>();
    private T prefab;
    private Transform root;

    public ObjectPool(T prefab, Transform root, int size)
    {
        this.prefab = prefab;
        this.root = root;

        for (int i = 0; i < size; i++)
        {
            var obj = Object.Instantiate(this.prefab, this.root);
            obj.gameObject.SetActive(false);
            stack.Push(obj);
        }
    }

    public T GetPool()
    {
        T obj;

        if (stack.Count > 0)
        {
            obj = stack.Pop();
        }
        else
        {
            obj = Object.Instantiate(prefab, root);
            obj.gameObject.SetActive(false);
        }

        obj.gameObject.SetActive(true);
        return obj;
    }

    public void ReturnPool(T obj)
    {
        obj.gameObject.SetActive(false);
        stack.Push(obj);
    }

    public void ClearPool()
    {
        foreach (var obj in stack)
        {
            if (obj != null)
                Object.Destroy(obj.gameObject);
        }

        stack.Clear();
    }
}