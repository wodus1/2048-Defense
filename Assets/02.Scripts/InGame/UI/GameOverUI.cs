using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour //게임 오버 ui view
{
    private GameManager gameManager;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button lobbyButton;
    public GameObject GameOverPanel;

    public void Initialize(GameManager gameManager)
    {
        this.gameManager = gameManager;

        restartButton.onClick.RemoveAllListeners();
        lobbyButton.onClick.RemoveAllListeners();

        restartButton.onClick.AddListener(OnClickRestartButton);
        lobbyButton.onClick.AddListener(OnClickLobbyButton);
    }

    private void OnClickRestartButton()
    {
        gameManager.InitGameSetting();
        GameOverPanel.SetActive(false);
    }

    private void OnClickLobbyButton()
    {
        SceneManager.LoadSceneAsync("Lobby");
    }
}
