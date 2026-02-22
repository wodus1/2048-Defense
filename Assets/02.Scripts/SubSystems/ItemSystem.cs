using UnityEngine;
using System.Collections.Generic;

public class ItemSystem : MonoBehaviour, ISubSystem // 아이템 시스템
{
    private GameManager gameManager;
    private PlayerStatsSystem playerStatsSystem;
    private MonsterSystem monsterSystem;
    private FXSystem fxSystem;

    [SerializeField] private List<ItemButtonUI> itemButtons = new List<ItemButtonUI>();

    public void Initialize(GameManager gameManager)
    {
        this.gameManager = gameManager;
        playerStatsSystem = this.gameManager.SubSystemsManager.GetSubSystem<PlayerStatsSystem>();
        monsterSystem = this.gameManager.SubSystemsManager.GetSubSystem<MonsterSystem>();
        fxSystem = this.gameManager.SubSystemsManager.GetSubSystem<FXSystem>();
        
        for (int i = 0; i < SaveManager.Instance.PlayerData.equipped.Length; i++)
        {
            string title = SaveManager.Instance.PlayerData.equipped[i];
            if (string.IsNullOrEmpty(title))
            {
                itemButtons[i].Initialize(this, null, 0);
            }
            else
            {
                itemButtons[i].Initialize(this, GlobalLibraryManager.Instance.GetItemTemplate(title), SaveManager.Instance.PlayerData.GetItemCount(title));
            }
        }
    }

    public void Deinitialize()
    {
        gameManager = null;
        playerStatsSystem = null;
        monsterSystem = null;
    }

    public void Execute(ItemEffect effect)
    {
        ItemUseContext itemUseContext = new ItemUseContext(playerStatsSystem, monsterSystem, fxSystem, new object());
        SaveManager.Instance.PlayerData.TryConsumeItem(effect.Title, 1);
        effect.Execute(itemUseContext);
    }
}