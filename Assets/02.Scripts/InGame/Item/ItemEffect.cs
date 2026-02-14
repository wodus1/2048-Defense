using UnityEngine;

public abstract class ItemEffect : ScriptableObject // 아이템 추상 클래스
{
    [SerializeField] private string title;
    [SerializeField] private string description;
    public string Title => title;
    public string Description => description;

    public abstract void Execute(ItemUseContext itemUseContext);
}