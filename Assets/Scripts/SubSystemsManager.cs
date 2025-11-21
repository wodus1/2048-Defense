using System.Collections.Generic;
using UnityEngine;

public class SubSystemsManager : MonoBehaviour
{
    [SerializeField] private Game2048Manager game2048Manager;
    private List<ISubSystem> subSystems = new List<ISubSystem>();

    void Awake()
    {
        var components = GetComponentsInChildren<MonoBehaviour>(true);
        
        foreach (var comp in components)
        {
            if (comp is ISubSystem system)
            {
                subSystems.Add(system);
                system.Initialize(game2048Manager);
            }
        }
    }

    public T GetSystem<T>() where T : class, ISubSystem
    {
        foreach (var system in subSystems)
            if (system is T typed)
                return typed;

        return null;
    }
}
