using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class PendingSave // 백업 모델
{
    public string json;
    public int baseRevision;
    public long createdAtUnix;
}

public class SaveService // 로컬, 서버 데이터 저장/로드 처리 로직
{
    private const string localDataKey = "PLAYER_DATA";
    private const string localPendingKey = "PENDING_SAVE_JSON";

    private const string dataKey = "PLAYER_DATA_V1";
    private const string revKey = "PLAYER_DATA_REV";

    public void SaveLocal(string playerDataJson, PendingSave pending)
    {
        PlayerPrefs.SetString(localDataKey, playerDataJson ?? "");
        PlayerPrefs.SetString(localPendingKey, pending != null ? JsonUtility.ToJson(pending) : "");
        PlayerPrefs.Save();
    }

    public (string playerDataJson, PendingSave pending) LoadLocal()
    {
        string data = PlayerPrefs.GetString(localDataKey, "");
        string pending_json = PlayerPrefs.GetString(localPendingKey, "");

        PendingSave pending = null;
        if (!string.IsNullOrEmpty(pending_json))
        {
            try { pending = JsonUtility.FromJson<PendingSave>(pending_json); }
            catch { pending = null; }
        }

        return (data, pending);
    }

    public Task<string> LoadServerAsync()
    {
        var tcs = new TaskCompletionSource<string>();

        PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
            result =>
            {
                if (result.Data != null && result.Data.TryGetValue(dataKey, out var v))
                    tcs.TrySetResult(v.Value ?? "");
                else
                    tcs.TrySetResult("");
            },
            error => tcs.TrySetException(new Exception(error.GenerateErrorReport()))
            );

        return tcs.Task;
    }

    public Task<(bool ok, int newRevision)> SaveServerAsync(string json, int baseRevision)
    {
        var tcs = new TaskCompletionSource<(bool ok, int newRevision)>();

        PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
            result =>
            {
                int serverRev = 0;
                if (result.Data != null && result.Data.TryGetValue(revKey, out var rv))
                    int.TryParse(rv.Value, out serverRev);

                if (serverRev != baseRevision)
                {
                    tcs.TrySetResult((false, serverRev));
                    return;
                }

                int newRev = serverRev + 1;

                var req = new UpdateUserDataRequest
                {
                    Data = new Dictionary<string, string>
                    {
                        { dataKey, json ?? "" },
                        { revKey, newRev.ToString() }
                    }
                };

                PlayFabClientAPI.UpdateUserData(req,
                    result => tcs.TrySetResult((true, newRev)),
                    error => tcs.TrySetException(new Exception(error.GenerateErrorReport()))
                );
            },
            error => tcs.TrySetException(new Exception(error.GenerateErrorReport()))
        );

        return tcs.Task;
    }
}