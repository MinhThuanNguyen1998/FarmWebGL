using Cysharp.Threading.Tasks;
using System;
using System.Data.Common;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;


public class UserDataService
{
    public UserData Data { get; private set; } = null;
    public bool IsLoaded { get; private set; } = false;

    [Inject] private readonly SignalBus m_SignalBus;

    public async UniTask<LoadDataResult> LoadAllDataAsync()
    {
        string url = ApiConfig.API_DATA_URL;
        string token = TokenManager.GetAccessToken();
        try
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.SetRequestHeader("Authorization", "Bearer " + token);
                await request.SendWebRequest().ToUniTask();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    string jsonResponse = request.downloadHandler.text;
                    Data = JsonUtility.FromJson<UserData>(jsonResponse);
                    //Debug.Log($"Raw JSON response: {jsonResponse}");
                    IsLoaded = true;
                    m_SignalBus.Fire(new UserDataLoadedSignal(Data));
                    return LoadDataResult.Success;
                }
                if (request.responseCode == 401) // Token expired or unauthorized
                {
                    Debug.LogWarning("Token expired (401 Unauthorized).");
                    return LoadDataResult.Unauthorized;
                }

                // Other errors
                Debug.LogError($"API Network Error: {request.error} (Code: {request.responseCode})");
                return LoadDataResult.FetchError;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error loading user data: {ex.Message}");
            return LoadDataResult.FetchError;
        }
    }
    public void ResetData()
    {
        IsLoaded = false;
        Data = null;
    }
}
