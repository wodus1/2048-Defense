using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsUI : MonoBehaviour
{
    private PlayerStatsSystem playerStatsSystem;

    [SerializeField] private GameObject statsPanel;
    [SerializeField] private TMP_Text attackValue;
    [SerializeField] private TMP_Text attackSpeedValue;
    [SerializeField] private TMP_Text projectileSpeedValue;
    [SerializeField] private Button closeButton;
    public Button StatsButton;

    public void Awake()
    {
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(OnClickCloseButton);
    }

    public void Initialize(PlayerStatsSystem playerStatsSystem, 
        float attackDamage, float attackSpeed, float projectileSpeed)
    {
        this.playerStatsSystem = playerStatsSystem;
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
}