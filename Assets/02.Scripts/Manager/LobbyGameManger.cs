using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyGameManger : MonoBehaviour // 로비 게임 매니저
{
    [SerializeField] private Canvas loginCanvas;
    [SerializeField] private Canvas lobbyCanvas;
    [SerializeField] private StoreController storeController;
    [SerializeField] private BagController bagController;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button startButton;
    [SerializeField] private Button homeButton;
    [SerializeField] private Button bagButton;
    [SerializeField] private Button storeButton;

    private bool isLogin = false;

    private void Start()
    {
        if (!PlayFabManager.Instance.IsLoggedIn)
        {
            lobbyCanvas.gameObject.SetActive(false);
            loginCanvas.gameObject.SetActive(true);
        }
        else
        {
            lobbyCanvas.gameObject.SetActive(true);
            loginCanvas.gameObject.SetActive(false);
        }

        loginButton.onClick.RemoveAllListeners();
        startButton.onClick.RemoveAllListeners();
        homeButton.onClick.RemoveAllListeners();
        bagButton.onClick.RemoveAllListeners();
        storeButton.onClick.RemoveAllListeners();

        loginButton.onClick.AddListener(OnClickGuestLogin);
        startButton.onClick.AddListener(OnClickStart);
        homeButton.onClick.AddListener(OnClickHome);
        storeButton.onClick.AddListener(OnClickStore);
        bagButton.onClick.AddListener(OnClickBag);

        PlayFabManager.Instance.OnLoginSuccess += OnLoginSuccess;

        storeController.GoldRefresh();
    }

    private void OnClickGuestLogin()
    {
        if (isLogin) return;

        isLogin = true;
        PlayFabManager.Instance.LoginGuest();
    }

    private async void OnLoginSuccess()
    {
        if (SaveManager.Instance != null)
        {
            await SaveManager.Instance.AfterLoginAsync();
        }

        lobbyCanvas.gameObject.SetActive(true);
        loginCanvas.gameObject.SetActive(false);

        isLogin = false;
    }

    private void OnClickHome()
    {
        PlayFabManager.Instance.Logout();

        lobbyCanvas.gameObject.SetActive(false);
        loginCanvas.gameObject.SetActive(true);
    }

    private void OnClickBag()
    {
        bagController.gameObject.SetActive(true);
    }

    private void OnClickStart()
    {
        SceneManager.LoadSceneAsync("Roguelike");
    }

    private void OnClickStore()
    {
        storeController.gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        PlayFabManager.Instance.OnLoginSuccess -= OnLoginSuccess;
    }
}