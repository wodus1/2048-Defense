using PlayFab;
using PlayFab.ClientModels;
using System;
using UnityEngine;

public class PlayFabManager : MonoBehaviour // 게스트 로그인 구현
{
    public static PlayFabManager Instance { get; private set; }

    private const string CustomIdKey = "CUSTOM_ID";
    [SerializeField] private bool isLoggedIn;
    private string playfabId;

    public bool IsLoggedIn => isLoggedIn;
    public string PlayFabId => playfabId;

    public event Action OnLoginSuccess;
    public event Action OnLoginFail;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    [ContextMenu("GuestLogin")]
    public void LoginGuest()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            Debug.LogError("PlayFab TitleId is empty. Set it in PlayFabSharedSettings.asset");
            isLoggedIn = false;
            OnLoginSuccess?.Invoke(); // 로그인 하지 않고 로컬 저장으로 플레이 진행
            return;
        }

        var customId = PlayerPrefs.GetString(CustomIdKey, "");
        if (string.IsNullOrEmpty(customId))
        {
            customId = Guid.NewGuid().ToString("N");
            PlayerPrefs.SetString(CustomIdKey, customId);
            PlayerPrefs.Save();
            Debug.Log($"Creat New Guest Id");
        }

        var req = new LoginWithCustomIDRequest
        {
            CustomId = customId,
            CreateAccount = true,
        };

        PlayFabClientAPI.LoginWithCustomID(req,
            result =>
            {
                isLoggedIn = true;
                playfabId = result.PlayFabId;

                Debug.Log($"Guest Login success. PlayFabId={PlayFabId}");
                OnLoginSuccess?.Invoke();
            },
            error =>
            {
                isLoggedIn = false;
                playfabId = null;
                Debug.LogError(error.GenerateErrorReport());
                OnLoginFail?.Invoke();
            });
    }

#if UNITY_EDITOR
    [ContextMenu("ClearGuestId")]
    private void CleearGuestId()
    {
        PlayerPrefs.DeleteKey(CustomIdKey);
        PlayerPrefs.Save();
        Debug.Log("CustomIdKey cleared");
    }
#endif
}