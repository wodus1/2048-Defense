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
        playFabManager.OnLoginSuccess += OnLoginSuccess;
    }

    private void OnClickGuestLogin()
    {
        playFabManager.LoginGuest();
    }

    private async void OnLoginSuccess()
    {
        if (SaveManager.Instance != null)
        {
            await SaveManager.Instance.AfterLoginAsync();
        }

        _= SceneManager.LoadSceneAsync("Roguelike");
    }


    private void OnDestroy()
    {
        playFabManager.OnLoginSuccess -= OnLoginSuccess;
    }
}