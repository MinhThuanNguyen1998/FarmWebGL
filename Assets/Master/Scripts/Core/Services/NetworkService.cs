using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkService
{
    private UnityWebRequest CreateBaseRequest(string url, string method)
    {
        var request = new UnityWebRequest(url, method);
        request.SetRequestHeader("Authorization", $"Bearer {TokenManager.GetAccessToken()}");
        return request;
    }
    public async UniTask<(LoadDataResult status, T responseData)> SendGetRequestAsync<T>(string url) where T : class
    {
        try
        {
            using var request = CreateBaseRequest(url, UnityWebRequest.kHttpVerbGET);
            request.downloadHandler = new DownloadHandlerBuffer();

            await request.SendWebRequest().ToUniTask();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonUtility.FromJson<T>(request.downloadHandler.text);
                return (LoadDataResult.Success, response);
            }

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
    public async UniTask<bool> SendPostRequestAsync<TRequest>(string url, TRequest body)
    {
        try
        {
            using var request = CreateBaseRequest(url, UnityWebRequest.kHttpVerbPOST);
            string jsonBody = JsonUtility.ToJson(body);

            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonBody));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            await request.SendWebRequest().ToUniTask();
            return request.result == UnityWebRequest.Result.Success;
        }
        catch (Exception ex)
        {
            Debug.LogError($"System error during POST: {ex.Message}");
            return false;
        }
    }
}
