using UnityEngine;

public abstract class Effect : ScriptableObject //강화 효과 추상 클래스
{
    [SerializeField] internal string title;
    [SerializeField] internal string description;
    [SerializeField] internal float value = 0.2f;
    public abstract void Apply(UpgradeSystem upgradeSystem);
}