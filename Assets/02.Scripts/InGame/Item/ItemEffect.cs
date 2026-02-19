using UnityEngine;

public abstract class ItemEffect : ScriptableObject // 아이템 추상 클래스
{
    [SerializeField] private Sprite item;
    [SerializeField] private string title;
    [SerializeField] private string description;
    [SerializeField] private string price;

    public string Title => title;
    public string Description => description;
    public Sprite Item => item;
    public string Price => price;

    public abstract void Execute(ItemUseContext itemUseContext);
}