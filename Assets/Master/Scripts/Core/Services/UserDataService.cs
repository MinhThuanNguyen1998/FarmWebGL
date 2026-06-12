using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;


public class UserDataService
{
    public UserData Data { get; private set; } = null;
    public bool IsLoaded { get; private set; } = false;

    private readonly SignalBus m_SignalBus;

    [Inject]
    public UserDataService(SignalBus signalBus)
    {
        m_SignalBus = signalBus;
    }

    public async UniTask<bool> LoadAllDataAsync()
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
                    return true;
                }
                else
                {
                    Debug.LogError($"API Error: {request.error}");
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error loading user data: {ex.Message}");
            return false;
        }
    }
    public void ResetData()
    {
        IsLoaded = false;
        Data = null;
    }
}
