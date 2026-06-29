using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;
public class UserDataService
{
    public UserData Data { get; private set; }
    public bool IsLoaded { get; private set; }

    [Inject] private readonly SignalBus m_SignalBus;

    public async UniTask<LoadDataResult> LoadAllDataAsync()
    {
        var (result, jsonResponse) = await SendGetRequestAsync(ApiConfig.API_GET_USER_DATA_URL);
        if (result == LoadDataResult.Success)
        {
            // Parse wrapper 
            var response = JsonUtility.FromJson<ApiUserDataResponse>(jsonResponse);

            if (response != null && response.success && response.data != null)
            {
                Data = new UserData
                {
                    userInfo = response.data.user,
                    farm = response.data.farm
                };
                IsLoaded = true;
                m_SignalBus.Fire(new UserDataLoadedSignal(Data));
            }
            else
            {
                Debug.LogError("[UserDataService] API success=false or data null.");
                return LoadDataResult.FetchError;
            }
        }
        return result;
    }

    public async UniTask<bool> AddAnimalAsync(string groupName)
    {
        string jsonBody = JsonUtility.ToJson(new AddAnimalRequest { groupName = groupName });
        if (await SendPostRequestAsync(ApiConfig.API_ADD_ANIMAL_URL, jsonBody))
        {
            Debug.Log($"Successfully added animal: '{groupName}'");
            return await LoadAllDataAsync() == LoadDataResult.Success;
        }
        return false;
    }

    public void ResetData()
    {
        IsLoaded = false;
        Data = null;
    }

    private UnityWebRequest CreateBaseRequest(string url, string method)
    {
        var request = new UnityWebRequest(url, method);
        request.SetRequestHeader("Authorization", $"Bearer {TokenManager.GetAccessToken()}");
        return request;
    }

    private async UniTask<(LoadDataResult status, string content)> SendGetRequestAsync(string url)
    {
        try
        {
            using var request = CreateBaseRequest(url, UnityWebRequest.kHttpVerbGET);
            request.downloadHandler = new DownloadHandlerBuffer();
            await request.SendWebRequest().ToUniTask();

            if (request.result == UnityWebRequest.Result.Success)
                return (LoadDataResult.Success, request.downloadHandler.text);

            if (request.responseCode == 401) Debug.LogWarning("Token expired (401 Unauthorized).");
            else Debug.LogError($"API GET Error: {request.error} (Code: {request.responseCode})");

            return (request.responseCode == 401 ? LoadDataResult.Unauthorized : LoadDataResult.FetchError, null);
        }
        catch (Exception ex)
        {
            Debug.LogError($"System error during GET: {ex.Message}");
            return (LoadDataResult.FetchError, null);
        }
    }

    private async UniTask<bool> SendPostRequestAsync(string url, string jsonBody)
    {
        try
        {
            using var request = CreateBaseRequest(url, UnityWebRequest.kHttpVerbPOST);
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonBody));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            await request.SendWebRequest().ToUniTask();
            if (request.result == UnityWebRequest.Result.Success) return true;

            Debug.LogError($"API POST Error: {request.error} (Code: {request.responseCode})");
            return false;
        }
        catch (Exception ex)
        {
            Debug.LogError($"System error during POST: {ex.Message}");
            return false;
        }
    }

    [Serializable] private class AddAnimalRequest { public string groupName; }
}