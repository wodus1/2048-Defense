using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyGameManger : MonoBehaviour // 로비 게임 매니저
{
    [SerializeField] Button startButton;

    private void Awake()
    {
        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(OnClickStart);
    }

    private void OnClickStart()
    {
        SceneManager.LoadSceneAsync("Roguelike");
    }
}
