using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour // 게임 오버 ui view
{
    private GameManager gameManager;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button lobbyButton;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text goldText;

    public void Initialize(GameManager gameManager)
    {
        this.gameManager = gameManager;

        restartButton.onClick.RemoveAllListeners();
        lobbyButton.onClick.RemoveAllListeners();

        restartButton.onClick.AddListener(OnClickRestartButton);
        lobbyButton.onClick.AddListener(OnClickLobbyButton);
    }

    public void SetOn(int gold)
    {
        SaveManager.Instance.PlayerData.AddGold(gold);
        goldText.SetText($"+{gold}");
        gameOverPanel.SetActive(true);
    }

    private void OnClickRestartButton()
    {
        gameManager.InitGameSetting();
        gameOverPanel.SetActive(false);
    }

    private void OnClickLobbyButton()
    {
        SceneManager.LoadSceneAsync("Lobby");
    }
}
