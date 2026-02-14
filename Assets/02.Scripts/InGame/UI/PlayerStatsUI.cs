using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsUI : MonoBehaviour // 플레이어 스탯 ui view
{
    private PlayerStatsSystem playerStatsSystem;

    [SerializeField] private GameObject statsPanel;
    [SerializeField] private TMP_Text attackValue;
    [SerializeField] private TMP_Text attackSpeedValue;
    [SerializeField] private TMP_Text projectileSpeedValue;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button statsButton;

    public void Awake()
    {
        closeButton.onClick.RemoveAllListeners();
        statsButton.onClick.RemoveAllListeners();

        closeButton.onClick.AddListener(OnClickCloseButton);
        statsButton.onClick.AddListener(OnClickStatsButton);
    }

    public void Initialize(PlayerStatsSystem playerStatsSystem)
    {
        this.playerStatsSystem = playerStatsSystem;
    }

    public void SetStatsUI(float attackDamage, float attackSpeed, float projectileSpeed)
    {
        statsPanel?.SetActive(true);

        attackValue.text = attackDamage.ToString("0.##");
        attackSpeedValue.text = attackSpeed.ToString("0.##");
        projectileSpeedValue.text = projectileSpeed.ToString("0.##");
    }

    private void OnClickCloseButton()
    {
        statsPanel?.SetActive(false);
        playerStatsSystem.Resume();
    }

    private void OnClickStatsButton()
    {
        playerStatsSystem.OnPlayerStatsUIStatsUI();
    }
}