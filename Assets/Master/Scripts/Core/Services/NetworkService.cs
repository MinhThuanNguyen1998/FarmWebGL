using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkService
{
    private UnityWebRequest CreateRequest(string url, string method, bool isAuthenticated = true)
    {
        var request = new UnityWebRequest(url, method);
        if (isAuthenticated)
        {
            request.SetRequestHeader("Authorization", $"Bearer {TokenManager.GetAccessToken()}");
        }
        return request;
    }

    public async UniTask<(LoadDataResult status, T responseData)> SendGetRequestAsync<T>(string url) where T : class
    {
        try
        {
            using var request = CreateRequest(url, UnityWebRequest.kHttpVerbGET);
            request.downloadHandler = new DownloadHandlerBuffer();

            await request.SendWebRequest().ToUniTask();

            if (request.result == UnityWebRequest.Result.Success)
                return (LoadDataResult.Success, JsonUtility.FromJson<T>(request.downloadHandler.text));

            if (request.responseCode == 401)
                Debug.LogWarning("Token expired (401 Unauthorized).");
            else
                Debug.LogError($"API GET Error: {request.error} (Code: {request.responseCode})");

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
            using var request = CreateRequest(url, UnityWebRequest.kHttpVerbPOST);
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(body));

            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
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

    public async UniTask<(bool networkSuccess, TResponse responseData)> SendPublicPostRequestAsync<TRequest, TResponse>(
        string url, TRequest body) where TResponse : class
    {
        try
        {
            using var request = CreateRequest(url, UnityWebRequest.kHttpVerbPOST, isAuthenticated: false);
            request.downloadHandler = new DownloadHandlerBuffer();

            // Setup Body & Headers
            if (body is AuthService.LoginRequest loginData)
            {
                WWWForm form = new WWWForm();
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

            // Send Request
            try { await request.SendWebRequest().ToUniTask(); }
            catch (Exception ex) { Debug.LogWarning($"SendWebRequest threw: {ex.Message}"); }

            // Handle Response
            bool isSuccess = request.result == UnityWebRequest.Result.Success;
            bool isProtocolError = request.result == UnityWebRequest.Result.ProtocolError;

            if ((isSuccess || isProtocolError) && !string.IsNullOrEmpty(request.downloadHandler?.text))
            {
                try
                {
                    var response = JsonUtility.FromJson<TResponse>(request.downloadHandler.text);
                    if (isProtocolError) Debug.LogWarning($"API POST Protocol Error (handled): Code {request.responseCode}");
                    return (true, response);
                }
                catch (Exception parseEx)
                {
                    Debug.LogError($"Failed to parse response body: {parseEx.Message}");
                }
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
