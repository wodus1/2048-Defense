using System.Collections.Generic;
using UnityEngine;
using System;

public class SubSystemsManager : MonoBehaviour //서브 시스템 매니저
{
    private GameManager gameManager;
    private Dictionary<Type, ISubSystem> subSystems = new Dictionary<Type, ISubSystem>();

    public void Initialize(GameManager gameManager)
    {
        this.gameManager = gameManager;

        var subSystems = GetComponentsInChildren<ISubSystem>(true);

        foreach (var subSystem in subSystems)
        {
            this.subSystems.Add(subSystem.GetType(), subSystem);
        }

        InitSubSystems();
    }

    public void Deinitialize()
    {
        foreach (var subSystem in subSystems.Values)
        {
            subSystem.Deinitialize();
        }

        subSystems.Clear();

        gameManager = null;
    }

    private void InitSubSystems()
    {
        foreach(var subsystem in subSystems.Values)
        {
            subsystem.Initialize(gameManager);
        }
    }
   
    public T GetSubSystem<T>() where T : ISubSystem
    {
        subSystems.TryGetValue(typeof(T), out var subSystem);
        return (T)subSystem;
    }
}