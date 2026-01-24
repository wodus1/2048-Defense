using UnityEngine;

public abstract class Effect : ScriptableObject //강화 효과 추상 클래스
{
    [SerializeField] private string title;
    [SerializeField] private string description;
    public string Title => title;
    public string Description => description;

    public abstract void Apply(PlayerStatsSystem upgradeSystem, object source);
}