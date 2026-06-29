using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkService
{
    private UnityWebRequest CreateAuthenticatedRequest(string url, string method)
    {
        var request = new UnityWebRequest(url, method);
        request.SetRequestHeader("Authorization", $"Bearer {TokenManager.GetAccessToken()}");
        return request;
    }

    private UnityWebRequest CreateUnauthenticatedRequest(string url, string method)
    {
        return new UnityWebRequest(url, method);
    }

    // Authenticated GET — dùng cho mọi API cần token
    public async UniTask<(LoadDataResult status, T responseData)> SendGetRequestAsync<T>(string url) where T : class
    {
        try
        {
            using var request = CreateAuthenticatedRequest(url, UnityWebRequest.kHttpVerbGET);
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

    // Authenticated POST — dùng cho các action cần token (vd: mua thú)
    public async UniTask<bool> SendPostRequestAsync<TRequest>(string url, TRequest body)
    {
        try
        {
            using var request = CreateAuthenticatedRequest(url, UnityWebRequest.kHttpVerbPOST);
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

    // Unauthenticated POST với response body — dùng cho login (chưa có token)
    public async UniTask<(bool networkSuccess, TResponse responseData)> SendPublicPostRequestAsync<TRequest, TResponse>(
        string url, TRequest body) where TResponse : class
    {
        try
        {
            using var request = CreateUnauthenticatedRequest(url, UnityWebRequest.kHttpVerbPOST);
            string jsonBody = JsonUtility.ToJson(body);

            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonBody));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            await request.SendWebRequest().ToUniTask();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonUtility.FromJson<TResponse>(request.downloadHandler.text);
                return (true, response);
            }

            Debug.LogError($"API POST Error: {request.error} (Code: {request.responseCode})");
            return (false, null);
        }
        catch (Exception ex)
        {
            Debug.LogError($"System error during public POST: {ex.Message}");
            return (false, null);
        }
    }
}
