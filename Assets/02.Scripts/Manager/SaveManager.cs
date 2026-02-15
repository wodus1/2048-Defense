using System;
using System.Threading.Tasks;
using UnityEngine;

public class SaveManager : MonoBehaviour // 데이터 관리 매지저
{
    public static SaveManager Instance { get; private set; }

    private SaveService saveService = new SaveService();

    public PlayerData PlayerData { get; private set; }
    public bool IsDirty { get; private set; }
    public bool IsSaving { get; private set; }
    public PendingSave Pending { get; private set; }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadLocalData();
        HookChanged();
    }

    public void LoadLocalData()
    {
        var (localJson, pending) = saveService.LoadLocal();
        Pending = pending;

        if (!string.IsNullOrEmpty(localJson))
            PlayerData = Deserialize(localJson) ?? new PlayerData();
        else
            PlayerData = new PlayerData();

        IsDirty = false;
        SaveLocal();
    }

    public async Task<bool?> LoadServerAsync()
    {
        if (!PlayFabManager.Instance.IsLoggedIn) return false;

        try
        {
            string json = await saveService.LoadServerAsync();
            if (string.IsNullOrEmpty(json))
                return false; // 서버에 데이터 없으면 로컬 유지(신규 계정)

            var server = Deserialize(json);
            if (server != null)
                ApplyServerData(server);

            return true;
        }
        catch
        {
            return null;
        }
    }

    public async Task FlushPendingAsync()
    {
        if (Pending == null) return;
        await SaveServerAsync();
    }

    public void SaveLocal()
    {
        string json = Serialize(PlayerData);
        saveService.SaveLocal(json, Pending);
    }

    public async Task<bool> SaveServerAsync()
    {
        if (IsSaving) return false;
        if (!IsDirty && Pending == null) return true;

        if (!PlayFabManager.Instance.IsLoggedIn || 
            Application.internetReachability == NetworkReachability.NotReachable)
        {
            EnsurePending();
            return false;
        }

        IsSaving = true;
        try
        {
            EnsurePending(); // 최신 스냅샷 준비

            var pending = Pending;
            var res = await saveService.SaveServerAsync(pending.json, pending.baseRevision);

            if (res.ok)
            {
                PlayerData.revision = res.newRevision;
                PlayerData.lastSavedAtUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                Pending = null;
                IsDirty = false;
                SaveLocal();
                return true;
            }

            //revision이 다를 시 서버 기준으로 맞춰 주고 로컬 데이터 다시 저장
            PlayerData.revision = res.newRevision;
            Pending = new PendingSave
            {
                json = Serialize(PlayerData),
                baseRevision = PlayerData.revision,
                createdAtUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };
            SaveLocal();

            var retry = await saveService.SaveServerAsync(Pending.json, Pending.baseRevision);
            if (retry.ok)
            {
                PlayerData.revision = retry.newRevision;
                PlayerData.lastSavedAtUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                Pending = null;
                IsDirty = false;
                SaveLocal();
                return true;
            }

            SaveLocal();
            return false;
        }
        catch
        {
            Debug.Log("서버 저장 실패");
            return false;
        }
        finally
        {
            IsSaving = false;
        }
    }

    public async Task AfterLoginAsync()
    {
        if (!PlayFabManager.Instance.IsLoggedIn) return;

        bool? hasServerData = await LoadServerAsync();

        if (hasServerData == false)
        {
            IsDirty = true;
            await SaveServerAsync();
        }

        await FlushPendingAsync();
    }

    private void HookChanged()
    {
        PlayerData.OnChanged -= MarkDirty;
        PlayerData.OnChanged += MarkDirty;
    }

    private void MarkDirty()
    {
        IsDirty = true;
        SaveLocal();
    }

    private void ApplyServerData(PlayerData serverData)
    {
        PlayerData = serverData;
        HookChanged();
        IsDirty = false;
        SaveLocal();
    }

    private void EnsurePending()
    {
        if (IsDirty || Pending == null)
        {
            Pending = new PendingSave
            {
                json = Serialize(PlayerData),
                baseRevision = PlayerData.revision,
                createdAtUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            SaveLocal();
        }
    }

    private string Serialize(PlayerData playerData)
    {
        var dto = PlayerDataDTO.From(playerData);
        return JsonUtility.ToJson(dto);
    }

    private PlayerData Deserialize(string json)
    {
        try
        {
            var dto = JsonUtility.FromJson<PlayerDataDTO>(json);
            return dto?.ToModel();
        }
        catch
        {
            return null;
        }
    }

    private async void OnApplicationQuit()
    {
        if (PlayFabManager.Instance.IsLoggedIn)
            await SaveServerAsync();
    }
}