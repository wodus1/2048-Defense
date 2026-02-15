using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyGameManger : MonoBehaviour // 로비 게임 매니저
{
    [SerializeField] PlayFabManager playFabManager;
    [SerializeField] Button startButton;

    private void Awake()
    {
        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(OnClickGuestLogin);
        playFabManager.OnLoginSuccess += LoadSceneAsync;
    }

    private void OnClickGuestLogin()
    {
        playFabManager.LoginGuest();
    }

    private void LoadSceneAsync()
    {
        SceneManager.LoadSceneAsync("Roguelike");
    }

    private void OnDestroy()
    {
        playFabManager.OnLoginSuccess -= LoadSceneAsync;
    }
}