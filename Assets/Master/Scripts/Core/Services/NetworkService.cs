using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkService
{
    private UnityWebRequest CreateRequest(string url, string method, object body = null, bool isAuthenticated = true)
    {
        var request = new UnityWebRequest(url, method) { downloadHandler = new DownloadHandlerBuffer() };

        if (isAuthenticated)
            request.SetRequestHeader("Authorization", $"Bearer {TokenManager.GetAccessToken()}");

        if (body == null) return request;

        if (body is AuthService.LoginRequest loginData)
        {
            var form = new WWWForm();
            form.AddField("username", loginData.username);
            form.AddField("password", loginData.password);
            request.uploadHandler = new UploadHandlerRaw(form.data);
            foreach (var header in form.headers) request.SetRequestHeader(header.Key, header.Value);
        }
        else
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(body));
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.SetRequestHeader("Content-Type", "application/json");
        }
        return request;
    }

    private async UniTask<(bool success, TResponse data)> SendPostCoreAsync<TRequest, TResponse>(string url, TRequest body, bool isAuthenticated) where TResponse : class
    {
        try
        {
            using var request = CreateRequest(url, UnityWebRequest.kHttpVerbPOST, body, isAuthenticated);
            try { await request.SendWebRequest().ToUniTask(); } catch { /* Ignore network abort exception */ }

            bool isSuccess = request.result == UnityWebRequest.Result.Success;
            bool isProtocolError = request.result == UnityWebRequest.Result.ProtocolError;

            if ((isSuccess || isProtocolError) && !string.IsNullOrEmpty(request.downloadHandler?.text))
            {
                try { return (true, JsonUtility.FromJson<TResponse>(request.downloadHandler.text)); }
                catch (Exception e) { Debug.LogError($"Parse error: {e.Message}"); }
            }

            LogNetworkError("POST", request);
            return (false, null);
        }
        catch (Exception ex)
        {
            Debug.LogError($"System error during POST: {ex.Message}");
            return (false, null);
        }
    }

    public async UniTask<(LoadDataResult status, T responseData)> SendGetRequestAsync<T>(string url) where T : class
    {
        try
        {
            using var request = CreateRequest(url, UnityWebRequest.kHttpVerbGET);
            await request.SendWebRequest().ToUniTask();

            if (request.result == UnityWebRequest.Result.Success)
                return (LoadDataResult.Success, JsonUtility.FromJson<T>(request.downloadHandler.text));

            LogNetworkError("GET", request);
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
        using var request = CreateRequest(url, UnityWebRequest.kHttpVerbPOST, body);
        try { await request.SendWebRequest().ToUniTask(); } catch { }
        return request.result == UnityWebRequest.Result.Success;
    }

    public UniTask<(bool networkSuccess, TResponse responseData)> SendAuthenticatedPostRequestAsync<TRequest, TResponse>(string url, TRequest body) where TResponse : class
        => SendPostCoreAsync<TRequest, TResponse>(url, body, isAuthenticated: true);
    public UniTask<(bool networkSuccess, TResponse responseData)> SendPublicPostRequestAsync<TRequest, TResponse>(string url, TRequest body) where TResponse : class
        => SendPostCoreAsync<TRequest, TResponse>(url, body, isAuthenticated: false);

    private void LogNetworkError(string method, UnityWebRequest request)
    {
        if (request.responseCode == 401) Debug.LogWarning("Token expired (401 Unauthorized).");
        else Debug.LogError($"API {method} Error: {request.error} (Code: {request.responseCode})");
    }
}
